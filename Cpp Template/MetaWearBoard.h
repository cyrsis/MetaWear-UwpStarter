#pragma once

#include "metawear/core/metawearboard.h"

namespace Cpp_Template {
    /// <summary>
    /// Wrapper class that Sets up the C++ API to be used in a C++ UWP app following the steps outlined in the C++ documentation:
    /// https://mbientlab.com/cppdocs/latest/btlecommunication.html
    /// </summary>
    class MetaWearBoard {
    public:
        /// <summary>
        /// Gets an instance of the MetaWearBoard class 
        /// </summary>
        /// <param name="device">C++ UWP BtleDevice object representing the MetaWear board</param>
        static MetaWearBoard* getInstance(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);

        virtual ~MetaWearBoard();

        /// <summary>
        /// Initialize the API
        /// </summary>
        /// <param name="initialized">Callback function alerting the caller when API initialization is done</param>
        void initialize(MblMwFnBoardPtrInt initialized);
        /// <summary>
        /// Free the memory allocated by the C++ API
        /// </summary>
        void free();

        /// <summary>
        /// Pointer representing the MblMwMetaWearBoard struct created by the C++ API, valid only after initialization succeeds
        /// </summary>
        MblMwMetaWearBoard *cppBoard;
    private:
        static MblMwBtleConnection conn;
        /// <summary>
        /// Writes a value to a GATT characteristic
        /// </summary>
        /// <param name="caller">Object calling this function</param>
        /// <param name="gattCharPtr">GATT Characteristic to write to</param>
        /// <param name="value">Pointer to a byte array containing the value</param>
        /// <param name="length">Number of bytes</param>
        static void writeGattCharacteristic(const void* caller, const MblMwGattChar* characteristic, const uint8_t* value, uint8_t length);
        /// <summary>
        /// Reads the value from a GATT characteristic
        /// </summary>
        /// <param name="caller">Object calling this function</param>
        /// <param name="characteristic">GATT characterirstic to read from</param>
        static void readGattCharacteristic(const void* caller, const MblMwGattChar* characteristic);

        MetaWearBoard(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);

        Windows::Devices::Bluetooth::BluetoothLEDevice ^device;
        /// <summary>
        /// GATT characteristic that MetaWear uses to send data to the host device
        /// </summary>
        Windows::Devices::Bluetooth::GenericAttributeProfile::GattCharacteristic^  mwNotifyGattChar;
    };
}