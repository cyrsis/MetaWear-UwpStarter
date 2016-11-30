//
// DeviceSetup.xaml.cpp
// Implementation of the DeviceSetup class
//

#include "pch.h"

#include "metawear/peripheral/led.h"

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

    MblMwLedPattern pattern;
    pattern.repeat_count = 5;

    mbl_mw_led_load_preset_pattern(&pattern, MBL_MW_LED_PRESET_PULSE);
    mbl_mw_led_write_pattern(wrapperBoard->cppBoard, &pattern, MBL_MW_LED_COLOR_GREEN);
    mbl_mw_led_play(wrapperBoard->cppBoard);
}


void Cpp_Template::DeviceSetup::back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
    mbl_mw_led_stop_and_clear(wrapperBoard->cppBoard);
    mbl_mw_metawearboard_tear_down(wrapperBoard->cppBoard);
    Frame->Navigate(MainPage::typeid);
}
