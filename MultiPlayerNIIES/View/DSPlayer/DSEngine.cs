using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DirectShowLib;


namespace MultiPlayerNIIES.View.DSPlayer
{
    public class DSEngine
    {
        private IFilterGraph2 m_FilterGraph;
        private IMediaControl m_mediaCtrl;
        private IMediaEvent m_mediaEvent;


        // Used to grab current snapshots
        ISampleGrabber m_sampGrabber = null;


    }
}
