#pragma once

#include "metawear/core/metawearboard.h"

namespace Cpp_Template {
    class MetaWearBoard {
    public:
        static MetaWearBoard* getInstance(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);

        virtual ~MetaWearBoard();

        void initialize(MblMwFnBoardPtrInt initialized);
        void free();
    private:
        static MblMwBtleConnection conn;
        static void writeGattCharacteristic(const void* caller, const MblMwGattChar* characteristic, const uint8_t* value, uint8_t length);
        static void readGattCharacteristic(const void* caller, const MblMwGattChar* characteristic);

        MetaWearBoard(Windows::Devices::Bluetooth::BluetoothLEDevice ^device);

        Windows::Devices::Bluetooth::BluetoothLEDevice ^device;
        MblMwMetaWearBoard *cppBoard;
        Windows::Devices::Bluetooth::GenericAttributeProfile::GattCharacteristic^  mwNotifyGattChar;
    };
}