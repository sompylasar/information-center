using System;
using System.IO;
using System.Collections.Generic;
using LogicUtils;

namespace InformationCenter.Services
{

    public class ByteBlockReader
    {

        #region Поля

        private Stream s = null;
        private byte[] buffer = null;
        private long blockSize = 1024;
        private List<byte[]> blocks = new List<byte[]>();

        #endregion

        #region Конструкторы

        public ByteBlockReader(Stream Stream, int BlockSize)
        {
            s = Stream;
            blockSize = BlockSize;
        }

        public ByteBlockReader(Stream Stream) : this(Stream, -1) { }

        #endregion

        #region Свойства

        public long BlockSize
        {
            get
            {
                if (blockSize == -1) blockSize = s.Length;
                if (blockSize == 0) blockSize = 1;
                return blockSize;
            }
        }

        public bool CanRead { get { return s.CanRead; } }

        #endregion

        #region Методы

        public int Read()
        {
            if (buffer == null) buffer = new byte[BlockSize];
            if (s.CanRead)
            {
                int readed = s.Read(buffer, 0, buffer.Length);
                if (readed > 0)
                {
                    byte[] block = new byte[readed];
                    STD.CopyBytes(block, buffer, 0, readed);
                    blocks.Add(block);
                }
                return readed;
            }
            else return 0;
        }

        public ByteBlockReader ReadToEnd() { while (Read() > 0) ; return this; }

        public byte[] ToArray()
        {
            int Length = 0;
            blocks.ForEach(new Action<byte[]>(b => Length += b.Length));
            byte[] array = new byte[Length];
            int current_offset = 0;
            for (int i = 0; i < blocks.Count; ++i)
            {
                STD.CopyBytes(array, blocks[i], current_offset, blocks[i].Length);
                current_offset += blocks[i].Length;
            }
            return array;
        }

        #endregion

    }

}