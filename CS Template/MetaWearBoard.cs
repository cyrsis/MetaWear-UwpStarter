using static MbientLab.MetaWear.Functions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices;
using Windows.Devices.Bluetooth;
using System.Runtime.InteropServices.WindowsRuntime;
using MbientLab.MetaWear.Core;

namespace MbientLab.MetaWear.Template {
    public sealed class MetaWearBoard {
        private static Dictionary<ulong, MetaWearBoard> instances= new Dictionary<ulong, MetaWearBoard>();
        public static async Task<MetaWearBoard> getMetaWearBoardInstance(BluetoothLEDevice btleDevice) {
            MetaWearBoard board;

            if (!instances.TryGetValue(btleDevice.BluetoothAddress, out board)) {
                await btleDevice.GetGattService(GattCharGuid.METAWEAR_NOTIFY_CHAR.serviceGuid).GetCharacteristics(GattCharGuid.METAWEAR_NOTIFY_CHAR.guid).FirstOrDefault()
                    .WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                board = new MetaWearBoard(btleDevice);
                instances.Add(btleDevice.BluetoothAddress, board);
            }
            return board;
        }

        private BluetoothLEDevice btleDevice;
        /// <summary>
        /// Pointer representing the MblMwMetaWearBoard struct created by the C++ API
        /// </summary>
        public IntPtr CppBoard { get; }
        /// <summary>
        /// C# wrapper around the MblMwBtleConnection struct 
        /// </summary>
        private BtleConnection btleConn;
        /// <summary>
        /// Delegate wrapper the <see cref="initialized"/> callback function
        /// </summary>
        private FnVoid initDelegate;

        private MetaWearBoard(BluetoothLEDevice btleDevice) {
            this.btleDevice = btleDevice;

            btleConn = new BtleConnection();
            btleConn.writeGattChar = new FnVoidPtrByteArray(writeCharacteristic);
            btleConn.readGattChar = new FnVoidPtr(readCharacteristic);

            var notifyChar = btleDevice.GetGattService(GattCharGuid.METAWEAR_NOTIFY_CHAR.serviceGuid).GetCharacteristics(GattCharGuid.METAWEAR_NOTIFY_CHAR.guid).FirstOrDefault();
            notifyChar.ValueChanged += notifyHandler;

            CppBoard = mbl_mw_metawearboard_create(ref btleConn);
            System.Diagnostics.Debug.WriteLine("board here: " + CppBoard);
        }

        public void Initialize(FnVoid initDelegate) {
            this.initDelegate = initDelegate;
            mbl_mw_metawearboard_initialize(CppBoard, this.initDelegate);
        }

        public void Free() {
            mbl_mw_metawearboard_free(CppBoard);

            var notifyChar = btleDevice.GetGattService(GattCharGuid.METAWEAR_NOTIFY_CHAR.serviceGuid).GetCharacteristics(GattCharGuid.METAWEAR_NOTIFY_CHAR.guid).FirstOrDefault();
            notifyChar.ValueChanged -= notifyHandler;

            instances.Remove(btleDevice.BluetoothAddress);
        }

        private void notifyHandler(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic gattCharChanged, GattValueChangedEventArgs obj) {
            byte[] response = obj.CharacteristicValue.ToArray();
            System.Diagnostics.Debug.WriteLine("board there: " + CppBoard);
            System.Diagnostics.Debug.WriteLine("response:" + response[0]);
            mbl_mw_connection_notify_char_changed(CppBoard, response, (byte)response.Length);
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
            var status = await btleDevice.GetGattService(charGuid.serviceGuid).GetCharacteristics(charGuid.guid).FirstOrDefault()
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
            var result = await btleDevice.GetGattService(charGuid.serviceGuid).GetCharacteristics(charGuid.guid).FirstOrDefault()
                .ReadValueAsync();

            if (result.Status == GattCommunicationStatus.Success) {
                mbl_mw_connection_char_read(CppBoard, gattCharPtr, result.Value.ToArray(), (byte)result.Value.Length);
            } else {
                System.Diagnostics.Debug.WriteLine("Error reading gatt characteristic");
            }
        }
    }
}
