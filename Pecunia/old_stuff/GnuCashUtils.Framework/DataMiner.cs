using System;
using System.Net;
using System.Text;

namespace GnuCashUtils.Framework
{
	public class DataMiner
	{
        public bool Eof
        {
            get
            {
                return !(responseCursor < responseString.Length && responseCursor != -1);
            }
        }

        public DataMiner (Uri miningUrl, string postData, Encoding encoding)
        {
            this.url = miningUrl;
            this.postData = postData;
            this.encoding = encoding;
        }

        public string ReadNext100Chars() {return responseString.Substring (responseCursor, 100); }

        public void FetchData ()
		{
            System.Net.WebRequest Req = System.Net.WebRequest.Create (url);

            if (postData != null)
            {
                Req.Method = "POST";
                Req.ContentLength = postData.Length;
                Req.ContentType = "application/x-www-form-urlencoded";

                using (System.IO.StreamWriter myWriter = new System.IO.StreamWriter (Req.GetRequestStream ()))
                    myWriter.Write (postData);
            }

            response = Req.GetResponse ();
            using (System.IO.StreamReader sr = new System.IO.StreamReader (response.GetResponseStream (), encoding))
            {
                responseString = sr.ReadToEnd ();
            }
		}

		public int SkipToText (string text)
		{
			if (Eof)
				throw new InvalidOperationException ("Unexpected end of document.");

			responseCursor = responseString.IndexOf (text, responseCursor);
			return responseCursor;
		}

		public int SkipOverText (string text)
		{
            if (text == null)
                throw new ArgumentNullException ("text");                
            
			SkipToText (text);
			
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			responseCursor += text.Length;
			return responseCursor;
		}

		public int SkipToNextLine()
		{
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			return SkipOverText ("\n");
		}

		public int SkipWhiteSpace()
		{
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			while (false == Eof && Char.IsWhiteSpace (responseString [responseCursor]))
				responseCursor++;

			return responseCursor;
		}

		public bool CheckText (string text)
		{
            if (text == null)
                throw new ArgumentNullException ("text");                
            
			if (IsNextText (text))
			{
				responseCursor += text.Length;
				return true;
			}

			return false;
		}

		public bool IsNextText (string text)
		{
            if (text == null)
                throw new ArgumentNullException ("text");                
            
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			if ((responseString.Substring (responseCursor, text.Length) == text))
				return true;

			return false;
		}

		public string FetchToText (string text)
		{
            if (text == null)
                throw new ArgumentNullException ("text");                
            
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			int begin = responseCursor;
			SkipToText (text);

			if (false == Eof)
			{
				int end = responseCursor;
				responseCursor += text.Length;
				return responseString.Substring (begin, end - begin);
			}

            throw new InvalidOperationException ("Unexpected end of document.");
		}

		public string FetchText (int length)
		{
			if (Eof)
                throw new InvalidOperationException ("Unexpected end of document.");

			string text = responseString.Substring (responseCursor, length);
			responseCursor += length;

			return text;
		}

		public int FindFirst (string[] possibilities)
		{
            if (possibilities == null)
                throw new ArgumentNullException ("possibilities");                
            
			int[] indices = new int [possibilities.Length];
			int whichFirst = -1;

			for (int i = 0; i < possibilities.Length; i++)
			{
				string variant = possibilities [i];
				indices [i] = responseString.IndexOf (variant, responseCursor);

				if (indices [i] != -1 && (whichFirst == -1 || (indices [i] > -1 && indices [i] < indices [whichFirst])))
					whichFirst = i;
			}

			if (whichFirst > -1)
				responseCursor = indices [whichFirst];

			return whichFirst;
		}

		private Uri url;
		private string postData;
        private Encoding encoding;
		private System.Net.WebResponse response;
		private string responseString;
		private int responseCursor = 0;
	}
}