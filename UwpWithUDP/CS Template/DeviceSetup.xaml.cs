using MbientLab.MetaWear.Core;
using static MbientLab.MetaWear.Functions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OSCForPCL;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MbientLab.MetaWear.Template
{
    /// <summary>
    /// Blank page where users add their MetaWear commands
    /// </summary>
    public sealed partial class DeviceSetup : Page
    {
        /// <summary>
        /// Pointer representing the MblMwMetaWearBoard struct created by the C++ API
        /// </summary>
        private IntPtr cppBoard;

        public FnVoidPtr accDataHandlerD;

       
        public static string ServerPort = "7474";

        static HostName ServerIP = new HostName("192.168.1.101");



        public DeviceSetup()
        {
            this.InitializeComponent();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var mwBoard = MetaWearBoard.getMetaWearBoardInstance(e.Parameter as BluetoothLEDevice);
            cppBoard = mwBoard.cppBoard;

            // cppBoard is initialized at this point and can be used
        }

        /// <summary>
        /// Callback for the back button which tears down the board and navigates back to the <see cref="MainPage"/> page
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            mbl_mw_metawearboard_tear_down(cppBoard);

            this.Frame.Navigate(typeof(MainPage));
        }

        private FnVoidPtr accDataHandler = new FnVoidPtr(dataPtr =>
        {
            Data marshalledData = Marshal.PtrToStructure<Data>(dataPtr);
            System.Diagnostics.Debug.WriteLine("Acc" + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value));
            var message = "Acc " +
                          Marshal
                              .PtrToStructure
                              <CartesianFloat>(
                                  marshalledData.value);

            Send(message);

        });

        private static void Send(string message)
        {
            OSCMessage messageOne = new OSCMessage(message); //one float argument
            //OSCMessage messageTwo = new OSCMessage("/address2", 5.0f, 3.2f, "attributes"); //two floats and a string

            OSCBundle bundle = new OSCBundle(messageOne);
            SendStringUdpAsync(ServerIP, ServerPort, bundle);
        }

        public static async Task SendStringUdpAsync(HostName remoteHost,
    string remotePort, OSCPacket packet)
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

            //Working
            //using (var socket = new DatagramSocket())
            //{
            //    var stream = (await socket.GetOutputStreamAsync(
            //        remoteHost, remotePort)).AsStreamForWrite();
            //    using (var writer = new StreamWriter(stream))
            //    {
            //        await writer.WriteLineAsync(message.ToString());
            //        await writer.FlushAsync();
            //    }
            //}
        }

        //        //HostName localhost= new HostName("127.0.0.1");

        //        //var socket = new DatagramSocket();
        //        //IAsyncAction connectAction = socket.ConnectAsync(localhost, "7474");
        //        //connectAction.AsTask().Wait();
    

    private FnVoidPtr barDataHandler = new FnVoidPtr(bardataPtr =>
    {
        Data marshalledData = Marshal.PtrToStructure<Data>(bardataPtr);
        System.Diagnostics.Debug.WriteLine("Bar" + Marshal.PtrToStructure<float>(marshalledData.value));

        var message = "Bar " + Marshal.PtrToStructure<float>(marshalledData.value);

        Send(message);
    });

    private FnVoidPtr GyroDataHandler = new FnVoidPtr(GyroDataPtr =>
    {
        Data marshalledData = Marshal.PtrToStructure<Data>(GyroDataPtr);
        System.Diagnostics.Debug.WriteLine("Gyro" + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value));

        var message = "Gyro " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value);
        Send(message);
    });

    private FnVoidPtr BieldData_handler = new FnVoidPtr(BieldDataPtr =>
    {
        Data marshalledData = Marshal.PtrToStructure<Data>(BieldDataPtr);
        System.Diagnostics.Debug.WriteLine("B-Field" + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value));
        var message = "B-Field " + Marshal.PtrToStructure<CartesianFloat>(marshalledData.value);
        Send(message);
    });


    private void accStart_Click(object sender, RoutedEventArgs e)
    {
        IntPtr accSignal = mbl_mw_acc_get_acceleration_data_signal(cppBoard);
        mbl_mw_datasignal_subscribe(accSignal, accDataHandler);
        mbl_mw_acc_enable_acceleration_sampling(cppBoard);
        mbl_mw_acc_start(cppBoard);

        IntPtr pa_signal = mbl_mw_baro_bosch_get_pressure_data_signal(cppBoard);
        mbl_mw_datasignal_subscribe(pa_signal, barDataHandler);
        mbl_mw_baro_bosch_start(cppBoard);

        IntPtr state_signal = mbl_mw_gyro_bmi160_get_rotation_data_signal(cppBoard);
        mbl_mw_datasignal_subscribe(state_signal, GyroDataHandler);
        mbl_mw_gyro_bmi160_enable_rotation_sampling(cppBoard);
        mbl_mw_gyro_bmi160_start(cppBoard);

        IntPtr bfield_signal = mbl_mw_mag_bmm150_get_b_field_data_signal(cppBoard);
        mbl_mw_datasignal_subscribe(bfield_signal, BieldData_handler);
        mbl_mw_mag_bmm150_enable_b_field_sampling(cppBoard);
        mbl_mw_mag_bmm150_start(cppBoard);

    }

    private void accStop_Click(object sender, RoutedEventArgs e)
    {
        IntPtr accSignal = mbl_mw_acc_get_acceleration_data_signal(cppBoard);
        IntPtr barSignal = mbl_mw_baro_bosch_get_pressure_data_signal(cppBoard);
        IntPtr gyroSignal = mbl_mw_gyro_bmi160_get_rotation_data_signal(cppBoard);
        IntPtr magSignal = mbl_mw_mag_bmm150_get_b_field_data_signal(cppBoard);

        mbl_mw_acc_stop(cppBoard);
        mbl_mw_acc_disable_acceleration_sampling(cppBoard);
        mbl_mw_datasignal_unsubscribe(accSignal);

        mbl_mw_baro_bosch_stop(cppBoard);
        mbl_mw_datasignal_unsubscribe(barSignal);

        mbl_mw_gyro_bmi160_stop(cppBoard);
        mbl_mw_gyro_bmi160_disable_rotation_sampling(cppBoard);
        mbl_mw_datasignal_unsubscribe(gyroSignal);


        mbl_mw_mag_bmm150_disable_b_field_sampling(cppBoard);
        mbl_mw_mag_bmm150_stop(cppBoard);
        mbl_mw_datasignal_unsubscribe(magSignal);

        //mbl_mw_metawearboard_free(cppBoard);







    }

}


}
