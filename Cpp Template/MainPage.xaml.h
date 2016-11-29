//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "MainPage.g.h"

namespace Cpp_Template
{
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
    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        void pairedDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
        void refreshDevices_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
