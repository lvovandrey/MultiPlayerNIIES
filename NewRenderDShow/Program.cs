//Don't forget to add reference to DirectShowLib in your project.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using DirectShowLib;


namespace NewRenderDShow
{

    namespace graphcode
    {
        class Program
        {
            static void checkHR(int hr, string msg)
            {
                if (hr < 0)
                {
                    Console.WriteLine(msg);
                    DsError.ThrowExceptionForHR(hr);
                }
            }

            static void BuildGraph(IGraphBuilder pGraph, string srcFile1)
            {
                int hr = 0;

                //graph builder
                ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = pBuilder.SetFiltergraph(pGraph);
                checkHR(hr, "Can't SetFiltergraph");

                Guid CLSID_LAVSplitterSource = new Guid("{B98D13E7-55DB-4385-A33D-09FD1BA26338}"); //LAVSplitter.ax
                Guid CLSID_LAVAudioDecoder = new Guid("{E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491}"); //LAVAudio.ax
                Guid CLSID_LAVVideoDecoder = new Guid("{EE30215D-164F-4A92-A4EB-9D4C13390F9F}"); //LAVVideo.ax
                Guid CLSID_VideoRenderer = new Guid("{B87BEB7B-8D29-423F-AE4D-6582C10175AC}"); //quartz.dll

                //add LAV Splitter Source
                IBaseFilter pLAVSplitterSource = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_LAVSplitterSource));
                hr = pGraph.AddFilter(pLAVSplitterSource, "LAV Splitter Source");
                checkHR(hr, "Can't add LAV Splitter Source to graph");
                //set source filename
                IFileSourceFilter pLAVSplitterSource_src = pLAVSplitterSource as IFileSourceFilter;
                if (pLAVSplitterSource_src == null)
                    checkHR(unchecked((int)0x80004002), "Can't get IFileSourceFilter");
                hr = pLAVSplitterSource_src.Load(srcFile1, null);
                checkHR(hr, "Can't load file");

                //add LAV Audio Decoder
                IBaseFilter pLAVAudioDecoder = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_LAVAudioDecoder));
                hr = pGraph.AddFilter(pLAVAudioDecoder, "LAV Audio Decoder");
                checkHR(hr, "Can't add LAV Audio Decoder to graph");

                //add LAV Video Decoder
                IBaseFilter pLAVVideoDecoder = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_LAVVideoDecoder));
                hr = pGraph.AddFilter(pLAVVideoDecoder, "LAV Video Decoder");
                checkHR(hr, "Can't add LAV Video Decoder to graph");

                //connect LAV Splitter Source and LAV Video Decoder
                hr = pGraph.ConnectDirect(GetPin(pLAVSplitterSource, "Video"), GetPin(pLAVVideoDecoder, "Input"), null);
                checkHR(hr, "Can't connect LAV Splitter Source and LAV Video Decoder");

                //connect LAV Splitter Source and LAV Audio Decoder
                hr = pGraph.ConnectDirect(GetPin(pLAVSplitterSource, "Audio"), GetPin(pLAVAudioDecoder, "Input"), null);
                checkHR(hr, "Can't connect LAV Splitter Source and LAV Audio Decoder");

                //add Video Renderer
                IBaseFilter pVideoRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_VideoRenderer));
                hr = pGraph.AddFilter(pVideoRenderer, "Video Renderer");
                checkHR(hr, "Can't add Video Renderer to graph");



                //add Default DirectSound Device
                IBaseFilter pDefaultDirectSoundDevice = (IBaseFilter)new DSoundRender();
                hr = pGraph.AddFilter(pDefaultDirectSoundDevice, "Default DirectSound Device");
                checkHR(hr, "Can't add Default DirectSound Device to graph");

                //connect LAV Video Decoder and Video Renderer
                hr = pGraph.ConnectDirect(GetPin(pLAVVideoDecoder, "Output"), GetPin(pVideoRenderer, "VMR Input0"), null);
                checkHR(hr, "Can't connect LAV Video Decoder and Video Renderer");

                //connect LAV Audio Decoder and Default DirectSound Device
                hr = pGraph.ConnectDirect(GetPin(pLAVAudioDecoder, "Output"), GetPin(pDefaultDirectSoundDevice, "Audio Input pin (rendered)"), null);
                checkHR(hr, "Can't connect LAV Audio Decoder and Default DirectSound Device");

            }

            static void Main(string[] args)
            {
                try
                {
                    IGraphBuilder graph = (IGraphBuilder)new FilterGraph();
                    Console.WriteLine("Building graph...");
                    BuildGraph(graph, @"J:\TORRENT\1. Фильмы\Mult.RU.320x240\03 Ежи и Петруччо\01 Когда-то.mp4");
                    Console.WriteLine("Running...");
                    IMediaControl mediaControl = (IMediaControl)graph;
                    IMediaEvent mediaEvent = (IMediaEvent)graph;
                    int hr = mediaControl.Run();
                    checkHR(hr, "Can't run the graph");
                    bool stop = false;
                    while (!stop)
                    {
                        System.Threading.Thread.Sleep(500);
                        Console.Write(".");
                        EventCode ev;
                        IntPtr p1, p2;
                       
                        System.Windows.Forms.Application.DoEvents();
                        while (mediaEvent.GetEvent(out ev, out p1, out p2, 0) == 0)
                        {
                            if (ev == EventCode.Complete || ev == EventCode.UserAbort)
                            {
                                Console.WriteLine("Done!");
                                stop = true;
                            }
                            else
                            if (ev == EventCode.ErrorAbort)
                            {
                                Console.WriteLine("An error occured: HRESULT={0:X}", p1);
                                mediaControl.Stop();
                                stop = true;
                            }
                            mediaEvent.FreeEventParams(ev, p1, p2);
                        }
                    }
                }
                catch (COMException ex)
                {
                    Console.WriteLine("COM error: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.ToString());
                }
            }
            static IPin GetPin(IBaseFilter filter, string pinname)
            {
                IEnumPins epins;
                int hr = filter.EnumPins(out epins);
                checkHR(hr, "Can't enumerate pins");
                IntPtr fetched = Marshal.AllocCoTaskMem(4);
                IPin[] pins = new IPin[1];
                while (epins.Next(1, pins, fetched) == 0)
                {
                    PinInfo pinfo;
                    pins[0].QueryPinInfo(out pinfo);
                    bool found = (pinfo.name == pinname);
                    DsUtils.FreePinInfo(pinfo);
                    if (found)
                        return pins[0];
                }
                checkHR(-1, "Pin not found");
                return null;
            }

        }
    }

}
