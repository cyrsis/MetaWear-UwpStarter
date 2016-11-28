#include "pch.h"

#include "MetaWearBoard.h"
#include "metawear/platform/btle_connection.h"

#include <unordered_map>

using namespace Platform;
using namespace Cpp_Template;
using namespace Windows::Devices::Bluetooth;
using namespace Windows::Devices::Bluetooth::GenericAttributeProfile;
using namespace Windows::Foundation;
using namespace Windows::Security::Cryptography;

using std::unordered_map;

static unordered_map<uint64, MetaWearBoard*> instances;
static unordered_map<const void*, MetaWearBoard*> cppToWrapper;
MblMwBtleConnection MetaWearBoard::conn = { MetaWearBoard::writeGattCharacteristic, MetaWearBoard::readGattCharacteristic };

static inline Guid byteArrayToGuid(const uint8_t* uuids) {
    Array<byte>^ lower = ref new Array<byte>(8);
    for (unsigned int i = 0; i < lower->Length; i++) {
        lower[i] = *(uuids + 128 + i);
    }

    return Guid(*((uint32*) (uuids)), *((uint16*) uuids + 2), *((uint16*)uuids + 3), lower);
}

void MetaWearBoard::writeGattCharacteristic(const void* caller, const MblMwGattChar* characteristic, const uint8_t* value, uint8_t length) {
    Array<byte>^ wrapper = ref new Array<byte>(length);
    for (uint8_t i = 0; i < length; i++) {
        wrapper[i] = value[i];
    }

    Guid service = byteArrayToGuid((const uint8_t*)characteristic), guidChar = byteArrayToGuid((const uint8_t*)(&characteristic->uuid_high));
    auto dest = cppToWrapper.at(caller)->device->GetGattService(service)->GetCharacteristics(guidChar)->First()->Current;
    concurrency::create_task(dest->WriteValueAsync(CryptographicBuffer::CreateFromByteArray(wrapper), GattWriteOption::WriteWithoutResponse)).then([](GattCommunicationStatus status) {
        if (status != GattCommunicationStatus::Success) {
            OutputDebugString(L"Could not write to device\r\n");
        }
    });
}

void MetaWearBoard::readGattCharacteristic(const void* caller, const MblMwGattChar* characteristic) {
    Guid service = byteArrayToGuid((const uint8_t*)characteristic), guidChar = byteArrayToGuid((const uint8_t*)(&characteristic->uuid_high));
    auto src = cppToWrapper.at(caller)->device->GetGattService(service)->GetCharacteristics(guidChar)->First()->Current;

    concurrency::create_task(src->ReadValueAsync()).then([&caller, &characteristic](GattReadResult^ result) {
        if (result->Status != GattCommunicationStatus::Success) {
            OutputDebugString(L"Could not read gatt characteristic\r\n");
        } else {
            Array<byte>^ value;
            CryptographicBuffer::CopyToByteArray(result->Value, &value);
            mbl_mw_metawearboard_char_read(cppToWrapper.at(caller)->cppBoard, characteristic, value->Data, value->Length);
        }
    });
}

MetaWearBoard* MetaWearBoard::getInstance(Windows::Devices::Bluetooth::BluetoothLEDevice ^device) {
    if (!instances.count(device->BluetoothAddress)) {
        instances.emplace(device->BluetoothAddress, new MetaWearBoard(device));
    }
    return instances.at(device->BluetoothAddress);
}

MetaWearBoard::MetaWearBoard(BluetoothLEDevice ^device) {
    this->device = device;
    this->cppBoard = mbl_mw_metawearboard_create(&conn);

    cppToWrapper.emplace(this->cppBoard, this);
    instances.emplace(device->BluetoothAddress, this);
}

MetaWearBoard::~MetaWearBoard() {
    this->device = nullptr;
    mbl_mw_metawearboard_free(this->cppBoard);
    this->cppBoard = nullptr;
}

void MetaWearBoard::initialize(MblMwFnBoardPtrInt initialized) {
    if (mwNotifyGattChar == nullptr) {
        Guid notifyService = byteArrayToGuid((const uint8_t*)&METAWEAR_SERVICE_NOTIFY_CHAR), notifyChar = byteArrayToGuid((const uint8_t*) &(METAWEAR_SERVICE_NOTIFY_CHAR.uuid_high));
        mwNotifyGattChar = device->GetGattService(notifyService)->GetCharacteristics(notifyChar)->First()->Current;
        mwNotifyGattChar->WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue::Notify);
        mwNotifyGattChar->ValueChanged += ref new TypedEventHandler<GattCharacteristic^, GattValueChangedEventArgs^>([this](GattCharacteristic^ sender, GattValueChangedEventArgs^ obj) -> void {
            Array<byte>^ valueArray = ref new Array<byte>(obj->CharacteristicValue->Length);
            CryptographicBuffer::CopyToByteArray(obj->CharacteristicValue, &valueArray);

            mbl_mw_metawearboard_notify_char_changed(cppBoard, valueArray->Data, valueArray->Length);
        });;
    }
    mbl_mw_metawearboard_initialize(cppBoard, initialized);
}
void MetaWearBoard::free() {
    device = nullptr;
    mwNotifyGattChar = nullptr;

    mbl_mw_metawearboard_free(cppBoard);
    
    instances.erase(device->BluetoothAddress);
    cppToWrapper.erase(cppBoard);
}