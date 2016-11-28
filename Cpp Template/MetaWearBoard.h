#pragma once

#include "metawear/core/metawearboard.h"

namespace Cpp_Template {
    public ref class MetaWearBoard : Platform::Object {
    public:
        virtual ~MetaWearBoard();

        void initialize();
        void free();
    private:
        static MblMwBtleConnection conn;
        static MetaWearBoard^ getInstance(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);
        static void writeGattCharacteristic(const void* caller, const MblMwGattChar* characteristic, const uint8_t* value, uint8_t length);
        static void readGattCharacteristic(const void* caller, const MblMwGattChar* characteristic);

        MetaWearBoard(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);

        Windows::Devices::Bluetooth::BluetoothLEDevice ^device;
        MblMwMetaWearBoard *cppBoard;
        Windows::Devices::Bluetooth::GenericAttributeProfile::GattCharacteristic^  mwNotifyGattChar;
    };
}