#C# Template
The C# template demonstrates how to call into the C++ API using the C# wrappers provided by the NuGet package.

# Examples
## LED Switch
For this demo, we will add a toggle switch to the layout that will turn on/off the LED.  First, open up the 
[DeviceSetup.xaml](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/CS%20Template/DeviceSetup.xaml) layout file and add
the following XAML: 

```xaml
<ToggleSwitch x:Name="ledSwitch" Header="LED" HorizontalAlignment="Stretch" Margin="10,10,10,0" 
              VerticalAlignment="Top" Toggled="ledSwitch_Toggled"/>
```

Next, implement the ``ledSwitch_Toggled`` function in the 
[ DeviceSetup.xaml.cs](https://github.com/mbientlab/MetaWear-UwpStarter/blob/master/CS%20Template/DeviceSetup.xaml.cs) file:

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
Finally, build the app and use the switch to turn on/off the LED.
