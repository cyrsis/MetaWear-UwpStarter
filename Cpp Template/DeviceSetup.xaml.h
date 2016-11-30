//
// DeviceSetup.xaml.h
// Declaration of the DeviceSetup class
//

#pragma once

#include "MetaWearBoard.h"
#include "DeviceSetup.g.h"

namespace Cpp_Template
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class DeviceSetup sealed
	{
	public:
		DeviceSetup();
    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MetaWearBoard* wrapperBoard;
        void back_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
