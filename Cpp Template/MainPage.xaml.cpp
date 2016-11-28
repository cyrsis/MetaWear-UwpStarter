//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

#include <cstring>
#include <iomanip>
#include <sstream>

using namespace Cpp_Template;

using namespace Platform;
using namespace Windows::Devices::Bluetooth;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
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

    wstring hexString = wstream.str();
    hexString.insert(2, L":");
    hexString.insert(5, L":");
    hexString.insert(8, L":");
    hexString.insert(11, L":");
    hexString.insert(14, L":");

    return ref new String(hexString.c_str());
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
    refreshDevices_Click(nullptr, nullptr);
}



void Cpp_Template::MainPage::pairedDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e) {
    BluetoothLEDevice^ selectedDevice = dynamic_cast<BluetoothLEDevice^>(sender);

    if (selectedDevice != nullptr) {
        initFlyout->ShowAt(pairedDevices);
        /*
        var board = MetaWearBoard.getMetaWearBoardInstance(selectedDevice);
        board.Initialize(new Fn_IntPtr_Int(async(caller, status) = > {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async() = > {
                initFlyout.Hide();

                if (status == Status.ERROR_TIMEOUT) {
                    await new ContentDialog(){
                        Title = "Error",
                        Content = "API initialization timed out.  Try re-pairing the MetaWear or moving it closer to the host device",
                        PrimaryButtonText = "OK"
                    }.ShowAsync();
                } else {
                    this.Frame.Navigate(typeof(DeviceSetup), selectedDevice);
                }
            });
        }));
        */
    }
}


void Cpp_Template::MainPage::refreshDevices_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
    pairedDevices->Items->Clear();
    concurrency::create_task(DeviceInformation::FindAllAsync(BluetoothLEDevice::GetDeviceSelector())).then([this](DeviceInformationCollection^ devicesInfo) -> void {
        for (auto info : devicesInfo) {
            concurrency::create_task(BluetoothLEDevice::FromIdAsync(info->Id)).then([this](BluetoothLEDevice^ device) -> void {
                pairedDevices->Items->Append(device);
            });
        }
    });
}
