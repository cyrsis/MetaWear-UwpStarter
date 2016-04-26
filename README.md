# MetaWear UWP Starter
This project provides a template for creating Universal Windows Platform app with the MetaWear C++ API.  The provided code sets up the 
C++ API to be used in a UWP app and handles all the Bluetooth LE functionality; all users need to do is add in their own UI elements 
and MetaWear code.

# Usage
User additions will mostly be added to the [DeviceSetup.xaml.cs](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/CS%20Template/DeviceSetup.xaml.cs) 
file and the [DeviceSetup.xaml](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/CS%20Template/DeviceSetup.xaml) 
layout file.  We will show how this is done by adding a switch that controls the LED using this app template.

## LED Switch
In the ``DeviceSetup.xaml`` layout file, we will add a toggle switch to turn on/off the LED.  

```xaml
<ToggleSwitch x:Name="ledSwitch" Header="LED" HorizontalAlignment="Stretch" Margin="10,10,10,0" 
              VerticalAlignment="Top" Toggled="ledSwitch_Toggled"/>
```

In the ``DeviceSetup.xaml.cs`` file, implement the ``ledSwitch_Toggled`` function to turn on/off the LED.

```c#
private void ledSwitch_Toggled(object sender, RoutedEventArgs e) {
    if (ledSwitch.IsOn) {
        Led.Pattern pattern = new Led.Pattern();
        mbl_mw_led_load_preset_pattern(ref pattern, Led.PatternPreset.SOLID);
        mbl_mw_led_write_pattern(cppBoard, ref pattern, Led.Color.BLUE);
        mbl_mw_led_play(cppBoard);
    } else {
        mbl_mw_led_stop_and_clear(cppBoard);
    }
}
```

After making your code changes, load the app on your phone and use the switch to turn on/off the LED.
