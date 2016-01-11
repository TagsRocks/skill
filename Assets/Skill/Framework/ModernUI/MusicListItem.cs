using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
namespace Skill.Framework.ModernUI
{
    public class MusicListItem : MonoBehaviour
    {
        public UnityEngine.UI.Toggle Checkmark;
        public UnityEngine.UI.Text Title;
        public UnityEngine.UI.Text Artist;
        public UnityEngine.UI.Image Background;
        public Color EvenBackColor = Color.white;
        public Color OddBackColor = Color.white;

        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public RectTransform RectTransform { get; private set; }
        public bool IsChecked { get { return Checkmark.isOn; } set { Checkmark.isOn = value; } }


        private int _Index;

        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                if (Background != null)
                {
                    if (_Index % 2 == 0)
                        Background.color = EvenBackColor;
                    else
                        Background.color = OddBackColor;
                }
            }
        }



        private string _Path;
        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                UpdateInfo();
            }
        }

        protected virtual void UpdateInfo()
        {
            if (_Path != null && System.IO.File.Exists(_Path))
            {
                Title.text = System.IO.Path.GetFileNameWithoutExtension(_Path);
                Artist.text = "unknown artis";

                using (FileStream fs = File.OpenRead(_Path))
                {
                    MusicInfo info = GetMusicInfo(fs);
                    if (!IsNullOrEmpty(info.Title))
                    {
                        Title.text = info.Title;
                        UnityEditor.EditorGUIUtility.systemCopyBuffer = Title.text;
                    }

                    if (!IsNullOrEmpty(info.Artist))
                        Artist.text = info.Artist;
                    else if (!IsNullOrEmpty(info.Album))
                        Artist.text = info.Album;
                }
            }
            else
            {
                Title.text = "unknown";
                Artist.text = "unknown artis";
            }
        }


        public static MusicInfo GetMusicInfo(FileStream stream)
        {
            MusicInfo info = new MusicInfo();

            if (stream.Length >= 128)
            {
                MusicID3Tag tag = new MusicID3Tag();

                stream.Seek(-128, SeekOrigin.End);
                stream.Read(tag.TAGID, 0, tag.TAGID.Length);
                string theTAGID = Encoding.Default.GetString(tag.TAGID);

                if (theTAGID.Equals("TAG")) // version 1
                {
                    stream.Read(tag.Title, 0, tag.Title.Length);
                    stream.Read(tag.Artist, 0, tag.Artist.Length);
                    stream.Read(tag.Album, 0, tag.Album.Length);
                    stream.Read(tag.Year, 0, tag.Year.Length);
                    stream.Read(tag.Comment, 0, tag.Comment.Length);
                    stream.Read(tag.Genre, 0, tag.Genre.Length);

                    info.Title = Encoding.Default.GetString(tag.Title);
                    info.Artist = Encoding.Default.GetString(tag.Artist);
                    info.Album = Encoding.Default.GetString(tag.Album);
                    info.Year = Encoding.Default.GetString(tag.Year);
                    info.Comment = Encoding.Default.GetString(tag.Comment);
                    info.Genre = Encoding.Default.GetString(tag.Genre);

                }
            }

            if (IsNullOrEmpty(info.Title)) info.Title = string.Empty;
            if (IsNullOrEmpty(info.Artist)) info.Artist = string.Empty;
            if (IsNullOrEmpty(info.Album)) info.Album = string.Empty;
            if (IsNullOrEmpty(info.Year)) info.Year = string.Empty;
            if (IsNullOrEmpty(info.Comment)) info.Comment = string.Empty;
            if (IsNullOrEmpty(info.Genre)) info.Genre = string.Empty;

            return info;
        }

        public static bool IsNullOrEmpty(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLetterOrDigit(str[i]) && !char.IsWhiteSpace(str[i]))
                    return false;
            }
            return true;
        }

        public class MusicInfo
        {
            public string Title = string.Empty;
            public string Artist = string.Empty;
            public string Album = string.Empty;
            public string Year = string.Empty;
            public string Comment = string.Empty;
            public string Genre = string.Empty;
        }

        class MusicID3Tag
        {
            public byte[] TAGID = new byte[3];      //  3
            public byte[] Title = new byte[30];     //  30
            public byte[] Artist = new byte[30];    //  30 
            public byte[] Album = new byte[30];     //  30 
            public byte[] Year = new byte[4];       //  4 
            public byte[] Comment = new byte[30];   //  30 
            public byte[] Genre = new byte[1];      //  1
        }
    }
}