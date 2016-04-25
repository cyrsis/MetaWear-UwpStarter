using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MbientLab.MetaWear.Template {
    public sealed class MacAddressHexString : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            string hexString = ((ulong)value).ToString("X");
            return hexString.Insert(2, ":").Insert(5, ":").Insert(8, ":").Insert(11, ":").Insert(14, ":");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public sealed class ConnectionStateColor : IValueConverter {
        public SolidColorBrush ConnectedColor { get; set; }
        public SolidColorBrush DisconnectedColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language) {
            switch ((BluetoothConnectionStatus)value) {
                case BluetoothConnectionStatus.Connected:
                    return ConnectedColor;
                case BluetoothConnectionStatus.Disconnected:
                    return DisconnectedColor;
                default:
                    throw new MissingMemberException("Unrecognized connection status: " + value.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            refreshDevices_Click(null, null);
        }

        private async void refreshDevices_Click(object sender, RoutedEventArgs e) {
            pairedDevices.Items.Clear();
            foreach (var info in await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector())) {
                pairedDevices.Items.Add(await BluetoothLEDevice.FromIdAsync(info.Id));
            }
        }

        private void pairedDevices_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            BluetoothLEDevice selected = ((ListView)sender).SelectedItem as BluetoothLEDevice;

            if (selected != null) {
                this.Frame.Navigate(typeof(DeviceSetup), selected);
            }
        }
    }
}
