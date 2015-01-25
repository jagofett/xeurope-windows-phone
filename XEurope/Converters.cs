
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XEurope
{
    static class Converters
    {
        public static byte[] ConvertStreamToBytes(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        /*public static void Mat_to_vector_vector_Point(Mat m, List<MatOfPoint> pts)
        {
            if (pts == null)
                throw new Exception("Output List can't be null");

            if (m == null)
                throw new Exception("Input Mat can't be null");

            List<Mat> mats = new List<Mat>(m.Rows);
            Mat_to_vector_Mat(m, mats);
        
            foreach (Mat mi in mats) 
            {
                MatOfPoint pt = new MatOfPoint(mi);
                pts.Add(pt);
            }
        }*/

        /*public static void Mat_to_vector_Mat(Mat m, List<Mat> mats)
        {
            if (mats == null)
                throw new Exception("mats == null");
            int count = m.Rows;
            if (MatrixType.S32C2 != m.Type || m.Cols != 1)
                throw new Exception("CvType.CV_32SC2 != m.type() ||  m.cols()!=1\n" + m);

            mats.Clear();
            int[] buff = new int[count * 2];
            buff = m.Get<int[]>(0, 0);
            for (int i = 0; i < count; i++)
            {
                long addr = (((long)buff[i * 2]) << 32) | (((long)buff[i * 2 + 1]) & 0xffffffffL);
                mats.Add(new Mat(new IntPtr(addr)));
            }
        }*/
    }
}
