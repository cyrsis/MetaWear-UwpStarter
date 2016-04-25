using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Processor;
using static MbientLab.MetaWear.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MbientLab.MetaWear.Template {
    /// <summary>
    /// Sets up the C++ API to be used in C# following the steps outlined in the C++ documentation:
    /// https://mbientlab.com/cppdocs/latest/btlecommunication.html
    /// </summary>
    public sealed partial class DeviceSetup : Page {
        /// <summary>
        /// Selected Bluetooth LE device from the paired devices list
        /// </summary>
        private BluetoothLEDevice selectedDevice;
        /// <summary>
        /// Delegate wrapper the <see cref="initialized"/> callback function
        /// </summary>
        private FnVoid initDelegate;
        /// <summary>
        /// Pointer representing the MblMwMetaWearBoard struct created by the C++ API
        /// </summary>
        private IntPtr board;
        /// <summary>
        /// C# wrapper around the MblMwBtleConnection struct 
        /// </summary>
        private BtleConnection btleConn;

        public DeviceSetup() {
            this.InitializeComponent();

            btleConn = new BtleConnection();
            btleConn.writeGattChar = new FnVoidPtrByteArray(writeCharacteristic);
            btleConn.readGattChar = new FnVoidPtr(readCharacteristic);

            initDelegate = new FnVoid(initialized);
        }

        /// <summary>
        /// Callback function executed when the `mbl_mw_metawearboard_initialize` function is finished
        /// </summary>
        /// <remarks>Do not use the <see cref="board"/> variable until this function is called</remarks>
        /// <seealso cref="mbl_mw_metawearboard_initialize"/>
        private void initialized() {
            // Configure your board here
            System.Diagnostics.Debug.WriteLine("API Initialized");
        }

        /// <summary>
        /// Writes a value to a GATT characteristic
        /// </summary>
        /// <param name="gattCharPtr">Pointer to a <see cref="MbientLab.MetaWear.Core.GattCharacteristic"/></param>
        /// <param name="value">Pointer to a byte array containing the value</param>
        /// <param name="length">Number of bytes</param>
        private async void writeCharacteristic(IntPtr gattCharPtr, IntPtr value, byte length) {
            byte[] managedArray = new byte[length];
            Marshal.Copy(value, managedArray, 0, length);

            var charGuid = Marshal.PtrToStructure<MbientLab.MetaWear.Core.GattCharacteristic>(gattCharPtr).toGattCharGuid();
            var status = await selectedDevice.GetGattService(charGuid.serviceGuid).GetCharacteristics(charGuid.guid).FirstOrDefault()
                .WriteValueAsync(managedArray.AsBuffer(), GattWriteOption.WriteWithoutResponse);

            if (status != GattCommunicationStatus.Success) {
                System.Diagnostics.Debug.WriteLine("Error writing gatt characteristic");
            }
        }
        /// <summary>
        /// Reads the value from a GATT characteristic
        /// </summary>
        /// <param name="gattCharPtr">Pointer to a <see cref="MbientLab.MetaWear.Core.GattCharacteristic"/></param>
        private async void readCharacteristic(IntPtr gattCharPtr) {
            var charGuid = Marshal.PtrToStructure<MbientLab.MetaWear.Core.GattCharacteristic>(gattCharPtr).toGattCharGuid();
            var result = await selectedDevice.GetGattService(charGuid.serviceGuid).GetCharacteristics(charGuid.guid).FirstOrDefault()
                .ReadValueAsync();

            if (result.Status == GattCommunicationStatus.Success) {
                mbl_mw_connection_char_read(board, gattCharPtr, result.Value.ToArray(), (byte)result.Value.Length);
            } else {
                System.Diagnostics.Debug.WriteLine("Error reading gatt characteristic");
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            selectedDevice = e.Parameter as BluetoothLEDevice;
            var notifyChar = selectedDevice.GetGattService(GattCharGuid.METAWEAR_NOTIFY_CHAR.serviceGuid).GetCharacteristics(GattCharGuid.METAWEAR_NOTIFY_CHAR.guid).FirstOrDefault();
            await notifyChar.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            notifyChar.ValueChanged += new TypedEventHandler<Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic, GattValueChangedEventArgs>(
                (Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic sender, GattValueChangedEventArgs obj) => {
                    byte[] response = obj.CharacteristicValue.ToArray();
                    mbl_mw_connection_notify_char_changed(board, response, (byte)response.Length);
                });

            board = mbl_mw_metawearboard_create(ref btleConn);
            mbl_mw_metawearboard_initialize(board, initDelegate);
        }

        private void back_Click(object sender, RoutedEventArgs e) {
            mbl_mw_metawearboard_tear_down(board);
            mbl_mw_metawearboard_free(board);

            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
