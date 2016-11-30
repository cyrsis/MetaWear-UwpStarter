#C++/CX Template
The C++/CX template demonstrates how to use the C++ API using the Microsoft CX extensions to the C++ language.  Please see the 
next section for an example on how to use this template.

# Examples
## LED Switch
For this demo, we will add a toggle switch to the layout that will turn on/off the LED.  First, open up the 
[DeviceSetup.xaml](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/Cpp%20Template/DeviceSetup.xaml) layout file and add 
the following XAML:  

```xaml
<ToggleSwitch x:Name="ledSwitch" Header="LED" HorizontalAlignment="Stretch" Margin="10,10,10,0" 
              VerticalAlignment="Top" Toggled="ledSwitch_Toggled"/>
```
Next, implement the ``ledSwitch_Toggled`` function in the 
[DeviceSetup.xaml.h](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/Cpp%20Template/DeviceSetup.xaml.h) and 
[DeviceSetup.xaml.cpp](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/Cpp%20Template/DeviceSetup.xaml.cpp) files:

```c++
// add function definition to DeviceSetup.xaml.h
void ledSwitch_Toggled(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
```
```c++
// include the led.h header file
#include "metawear/peripheral/led.h"

// add function implementation to DeviceSetup.xaml.cpp
void Cpp_Template::DeviceSetup::ledSwitch_Toggled(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) {
    if (ledSwitch->IsOn) {
        MblMwLedPattern pattern;
        pattern.repeat_count = MBL_MW_LED_REPEAT_INDEFINITELY;

        mbl_mw_led_load_preset_pattern(&pattern, MBL_MW_LED_PRESET_SOLID);
        mbl_mw_led_write_pattern(wrapperBoard->cppBoard, &pattern, MBL_MW_LED_COLOR_BLUE);
        mbl_mw_led_play(wrapperBoard->cppBoard);
    } else {
        mbl_mw_led_stop_and_clear(wrapperBoard->cppBoard);
    }
}
```

Finally, build the app and use the switch to turn on/off the LED.
