using MbientLab.MetaWear.Peripheral;
using static MbientLab.MetaWear.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using MbientLab.MetaWear.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Storage.Streams;
using MbientLab.MetaWear.Sensor;
using OSCForPCL;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MbientLab.MetaWear.Template {
    /// <summary>
    /// Blank page where users add their MetaWear commands
    /// </summary>
    public sealed partial class DeviceSetup : Page {
        /// <summary>
        /// Pointer representing the MblMwMetaWearBoard struct created by the C++ API
        /// </summary>
        private IntPtr cppBoard;
        


        public static string ServerPort = "7474";

        static HostName ServerIP = new HostName("192.168.0.151");

        public DeviceSetup() {
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            var mwBoard= MetaWearBoard.getMetaWearBoardInstance(e.Parameter as BluetoothLEDevice);
            cppBoard = mwBoard.cppBoard;

            // cppBoard is initialized at this point and can be used
        }

        private Fn_IntPtr accDataHandler = new Fn_IntPtr(pointer => {
            Data data = Marshal.PtrToStructure<Data>(pointer);
            System.Diagnostics.Debug.WriteLine("ACC "+Marshal.PtrToStructure<CartesianFloat>(data.value));
        });


        private Fn_IntPtr barDataHandler = new Fn_IntPtr(bardataPtr =>
        {
            Data marshalledData = Marshal.PtrToStructure<Data>(bardataPtr);
            System.Diagnostics.Debug.WriteLine("Bar " + Marshal.PtrToStructure<float>(marshalledData.value));

            var message = "Bar " + Marshal.PtrToStructure<float>(marshalledData.value);

            //Send(message);
        });

        private Fn_IntPtr GyroDataHandler = new Fn_IntPtr(GyroDataPtr =>
        {
            Data marshalledData = Marshal.PtrToStructure<Data>(GyroDataPtr);
            System.Diagnostics.Debug.WriteLine("Gyro " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value));

            var message = "Gyro " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value);
            //Send(message);
        });

        private Fn_IntPtr BieldData_handler = new Fn_IntPtr(BieldDataPtr =>
        {
            Data marshalledData = Marshal.PtrToStructure<Data>(BieldDataPtr);
            System.Diagnostics.Debug.WriteLine("B-Field " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value));
            var message = "B-Field " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value);
            //Send(message);
        });

        private Fn_IntPtr fusionAccHandler = new Fn_IntPtr(FusionAccDataPtr =>
        {
            
            Data marshalledData = Marshal.PtrToStructure<Data>(FusionAccDataPtr);
            System.Diagnostics.Debug.WriteLine(DateTime.Now+ " Fusion  " + Marshal.PtrToStructure<Quaternion>(marshalledData.value));
            var message = "Fussion " + Marshal.PtrToStructure<Quaternion>(marshalledData.value).ToString();
            Send(message);
        });


        private void accStart_Click(object sender, RoutedEventArgs e)
        {

            ServerPort = ServiceNameForConnect.Text;

            ServerIP = new HostName(HostNameForConnect.Text);


            mbl_mw_sensor_fusion_set_mode(cppBoard, SensorFusion.Mode.NDOF);
            mbl_mw_sensor_fusion_set_acc_range(cppBoard, SensorFusion.AccRange.AR_4G);
            mbl_mw_sensor_fusion_write_config(cppBoard);

            IntPtr fussionsAccSignal = mbl_mw_sensor_fusion_get_data_signal(cppBoard,SensorFusion.Data.QUATERION);
            mbl_mw_datasignal_subscribe(fussionsAccSignal, fusionAccHandler);
            mbl_mw_sensor_fusion_enable_data(cppBoard, SensorFusion.Data.QUATERION);
            mbl_mw_sensor_fusion_start(cppBoard);

            //IntPtr accSignal = mbl_mw_acc_get_acceleration_data_signal(cppBoard);
            //mbl_mw_datasignal_subscribe(accSignal, accDataHandler);
            //mbl_mw_acc_enable_acceleration_sampling(cppBoard);
            //mbl_mw_acc_start(cppBoard);

            //IntPtr pa_signal = mbl_mw_baro_bosch_get_pressure_data_signal(cppBoard);
            //mbl_mw_datasignal_subscribe(pa_signal, barDataHandler);
            //mbl_mw_baro_bosch_start(cppBoard);

            //IntPtr state_signal = mbl_mw_gyro_bmi160_get_rotation_data_signal(cppBoard);
            //mbl_mw_datasignal_subscribe(state_signal, GyroDataHandler);
            //mbl_mw_gyro_bmi160_enable_rotation_sampling(cppBoard);
            //mbl_mw_gyro_bmi160_start(cppBoard);

            //IntPtr bfield_signal = mbl_mw_mag_bmm150_get_b_field_data_signal(cppBoard);
            //mbl_mw_datasignal_subscribe(bfield_signal, BieldData_handler);
            //mbl_mw_mag_bmm150_enable_b_field_sampling(cppBoard);
            //mbl_mw_mag_bmm150_start(cppBoard);

        }

        private void accStop_Click(object sender, RoutedEventArgs e)
        {

            mbl_mw_sensor_fusion_stop(cppBoard);

            //IntPtr accSignal = mbl_mw_acc_get_acceleration_data_signal(cppBoard);
            //IntPtr barSignal = mbl_mw_baro_bosch_get_pressure_data_signal(cppBoard);
            //IntPtr gyroSignal = mbl_mw_gyro_bmi160_get_rotation_data_signal(cppBoard);
            //IntPtr magSignal = mbl_mw_mag_bmm150_get_b_field_data_signal(cppBoard);

            //mbl_mw_acc_stop(cppBoard);
            //mbl_mw_acc_disable_acceleration_sampling(cppBoard);
            //mbl_mw_datasignal_unsubscribe(accSignal);

            //mbl_mw_baro_bosch_stop(cppBoard);
            //mbl_mw_datasignal_unsubscribe(barSignal);

            //mbl_mw_gyro_bmi160_stop(cppBoard);
            //mbl_mw_gyro_bmi160_disable_rotation_sampling(cppBoard);
            //mbl_mw_datasignal_unsubscribe(gyroSignal);


            //mbl_mw_mag_bmm150_disable_b_field_sampling(cppBoard);
            //mbl_mw_mag_bmm150_stop(cppBoard);
            //mbl_mw_datasignal_unsubscribe(magSignal);
        }

        private static void Send(string message)
        {
            OSCMessage messageOne = new OSCMessage(message); //one float argument
            //OSCMessage messageTwo = new OSCMessage("/address2", 5.0f, 3.2f, "attributes"); //two floats and a string

            OSCBundle bundle = new OSCBundle(messageOne);
           
            SendStringUdpAsync(ServerIP, ServerPort, bundle);
        }

        /// <summary>
        /// Callback for the back button which tears down the board and navigates back to the <see cref="MainPage"/> page
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e) {
            mbl_mw_metawearboard_tear_down(cppBoard);

            this.Frame.Navigate(typeof(MainPage));
        }

        public static  async Task SendStringUdpAsync(HostName remoteHost,string remotePort, OSCPacket packet)
        {
            
            var socket = new DatagramSocket();

            //socket.MessageReceived += SocketOnMessageReceived;

            using (var stream = await socket.GetOutputStreamAsync(remoteHost, remotePort))
            {
                using (var writer = new DataWriter(stream))
                {
                    // var data = Encoding.UTF8.GetBytes(message);

                    writer.WriteBytes(packet.Bytes);
                    writer.StoreAsync();
                }
            }
        }
        }
}
