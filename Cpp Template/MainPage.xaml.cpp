//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

#include "DeviceSetup.xaml.h"
#include "MetaWearBoard.h"

#include "metawear/core/status.h"

#include <cstring>
#include <iomanip>
#include <sstream>

using namespace Cpp_Template;

using namespace Platform;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Devices::Bluetooth;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

using std::hex;
using std::wstring;
using std::wstringstream;

using Windows::UI::Colors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

MacAddressHexString::MacAddressHexString() {

}

MacAddressHexString::~MacAddressHexString() {

}

Platform::Object ^ MacAddressHexString::Convert(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language) {
    wstringstream wstream;
    wstream << hex << (uint64) value;

    return ref new String(wstream.str()
        .insert(2, L":")
        .insert(5, L":")
        .insert(8, L":")
        .insert(11, L":")
        .insert(14, L":")
        .c_str()
    );
}

Platform::Object ^ MacAddressHexString::ConvertBack(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language) {
    throw ref new Platform::NotImplementedException();
}

ConnectionStateColor::ConnectionStateColor() {

}

ConnectionStateColor::~ConnectionStateColor() {

}

Platform::Object ^ ConnectionStateColor::Convert(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language) {
    switch ((BluetoothConnectionStatus)value) {
    case BluetoothConnectionStatus::Connected:
        return ref new SolidColorBrush(Colors::Green);
    case BluetoothConnectionStatus::Disconnected:
        return ref new SolidColorBrush(Colors::Red);
    default:
        throw ref new Platform::InvalidArgumentException("Unrecognized connection status: " + value->ToString());
    }
}

Platform::Object ^ ConnectionStateColor::ConvertBack(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language) {
    throw ref new Platform::NotImplementedException();
}

MainPage::MainPage()
{
	InitializeComponent();
}

void MainPage::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) {
    selectedDevice = nullptr;
    refreshDevices_Click(nullptr, nullptr);
}

void MainPage::HideInitFlyout() {
    initFlyout->Hide();
}

void MainPage::navigateToDeviceSetup() {
    Frame->Navigate(DeviceSetup::typeid, selectedDevice);
}

void Cpp_Template::MainPage::pairedDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e) {
    selectedDevice = dynamic_cast<BindableBtleDevice^>(dynamic_cast<ListView^>(sender)->SelectedItem)->device;

    if (selectedDevice != nullptr) {
        initFlyout->ShowAt(pairedDevices);

        auto board = MetaWearBoard::getInstance(selectedDevice);
        board->initialize([](MblMwMetaWearBoard* board, int32_t status) -> void {
            CoreApplication::MainView->CoreWindow->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([&status]() -> void {
                Windows::UI::Xaml::Controls::Frame^ frame = dynamic_cast<Windows::UI::Xaml::Controls::Frame^>(Window::Current->Content);
                auto page = dynamic_cast<MainPage^>(frame->Content);

                page->HideInitFlyout();

                if (status == MBL_MW_STATUS_ERROR_TIMEOUT) {
                    auto dialog = ref new ContentDialog();
                    dialog->Title = "Error";
                    dialog->Content = "API initialization timed out.  Try re-pairing the MetaWear or moving it closer to the host device";
                    dialog->PrimaryButtonText = "OK";
                    dialog->ShowAsync();
                } else {
                    page->navigateToDeviceSetup();
                }
            }));
        });
    }
}

void Cpp_Template::MainPage::refreshDevices_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
    pairedDevices->Items->Clear();
    concurrency::create_task(DeviceInformation::FindAllAsync(BluetoothLEDevice::GetDeviceSelector())).then([this](DeviceInformationCollection^ devicesInfo) -> void {
        for (auto info : devicesInfo) {
            concurrency::create_task(BluetoothLEDevice::FromIdAsync(info->Id)).then([this](BluetoothLEDevice^ device) -> void {
                pairedDevices->Items->Append(ref new BindableBtleDevice(device));
            });
        }
    });
}
