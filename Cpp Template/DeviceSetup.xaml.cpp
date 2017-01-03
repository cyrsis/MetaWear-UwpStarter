//
// DeviceSetup.xaml.cpp
// Implementation of the DeviceSetup class
//

#include "pch.h"

#include "MainPage.xaml.h"
#include "DeviceSetup.xaml.h"

using namespace Cpp_Template;

using namespace Platform;
using namespace Windows::Devices::Bluetooth;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

DeviceSetup::DeviceSetup()
{
	InitializeComponent();
}

void Cpp_Template::DeviceSetup::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs ^ e) {
    wrapperBoard = MetaWearBoard::getInstance(dynamic_cast<BluetoothLEDevice^>(e->Parameter));
    // use 'wrapperBoard->cppBoard' for all function calls to the C++ api
}

void Cpp_Template::DeviceSetup::back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
    mbl_mw_metawearboard_tear_down(wrapperBoard->cppBoard);
    Frame->Navigate(MainPage::typeid);
}