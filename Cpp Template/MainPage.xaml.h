﻿//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

namespace Cpp_Template
{

    [Windows::UI::Xaml::Data::BindableAttribute]
    public ref class BindableBtleDevice sealed {
        Windows::Devices::Bluetooth::BluetoothLEDevice^ _device;
    public:
        BindableBtleDevice(Windows::Devices::Bluetooth::BluetoothLEDevice^ device) {
            this->_device = device;
        }

        property Windows::Devices::Bluetooth::BluetoothLEDevice^ device {
            Windows::Devices::Bluetooth::BluetoothLEDevice^ get() { return _device; }
        }
        property Platform::String^ Name {
            Platform::String^ get() { return _device->Name; }
        }
        property uint64 BluetoothAddress {
            uint64 get() { return _device->BluetoothAddress; }
        }
        property Windows::Devices::Bluetooth::BluetoothConnectionStatus ConnectionStatus {
            Windows::Devices::Bluetooth::BluetoothConnectionStatus get() { return _device->ConnectionStatus; }
        }
    };

    public ref class MacAddressHexString sealed : Windows::UI::Xaml::Data::IValueConverter {
    public:
        MacAddressHexString();
        virtual ~MacAddressHexString();

        // Inherited via IValueConverter
        virtual Platform::Object ^ Convert(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language);
        virtual Platform::Object ^ ConvertBack(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language);
    };

    public ref class ConnectionStateColor sealed : Windows::UI::Xaml::Data::IValueConverter {
    public:
        ConnectionStateColor();
        virtual ~ConnectionStateColor();

        // Inherited via IValueConverter
        virtual Platform::Object ^ Convert(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language);
        virtual Platform::Object ^ ConvertBack(Platform::Object ^value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object ^parameter, Platform::String ^language);
    };


	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

        void HideInitFlyout();
        void navigateToDeviceSetup();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        Windows::Devices::Bluetooth::BluetoothLEDevice^ selectedDevice;
        void pairedDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
        void refreshDevices_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
