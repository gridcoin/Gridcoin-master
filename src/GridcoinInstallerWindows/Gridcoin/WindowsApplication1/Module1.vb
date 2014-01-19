Imports OpenCL.Net


Module CLDevices

    Public Structure GPUDevice
        Public GPUName As String
        Public Id As Long


    End Structure

    Public Function GetCLDevices() As List(Of GPUDevice)


        Stop




        Dim err As ErrorCode
        Dim platforms() As Platform
        Dim platform As Platform
        platforms = Cl.GetPlatformIDs(err)
        Dim devicesList As New List(Of Device)
        Dim gpulist As New List(Of GPUDevice)
        For Each platform In platforms
            Dim platformname As String = Cl.GetPlatformInfo(platform, PlatformInfo.Name, err).ToString()
            For Each device As OpenCL.Net.Device In Cl.GetDeviceIDs(platform, DeviceType.Gpu, err)

                '    CheckErr(error, "Cl.GetDeviceIDs");
                '                    Console.WriteLine("Device: " + device.ToString());
                devicesList.Add(device)
                Dim g As New GPUDevice
                g.GPUName = device.ToString
                g.Id = 1

                Debug.Print(device.ToString())

            Next
        Next

        '            //ToDO:Log Platform Device Name;
        '            //		status = clGetPlatformInfo(platform, CL_PLATFORM_NAME, sizeof(pbuff), pbuff, NULL);
        '            //		status = clGetPlatformInfo(platform, CL_PLATFORM_VERSION, sizeof(pbuff), pbuff, NULL);
        '            //		status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_GPU, 0, NULL, &numDevices);
        '            //ToDO: Saved the compiled version so it can be built the next time as a binary:
        '            //ToDO:  Enable certain cards, fan control, scheduling


        '                Console.WriteLine("Platform: " + platformName);

        '            //We will be looking only for GPU devices


    End Function

End Module
