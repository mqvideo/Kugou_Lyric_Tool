using Microsoft.VisualBasic;
using System.IO.Compression;
using System.Text;
namespace LKRC_Tool
{
    public partial class Form1 : Form
    {
        private static readonly byte[] KEYS = { 64, 71, 97, 119, 94, 50, 116, 71, 81, 54, 49, 45, 206, 210, 110, 105 };
        private int successCount = 0;
        private int failCount = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // 注册文件拖放对象
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            successCount = 0;
            failCount = 0;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            bool hasSupportedFiles = false;
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]).ToLower();
                if (ext == ".krc" || ext == ".lrc")
                {
                    hasSupportedFiles = true;
                    if (ext == ".krc") { krc2lrc(files[i]); }
                    else if (ext == ".lrc") { lrc2krc(files[i]); }
                }
            }
            if (hasSupportedFiles) { label2.Text = $"成功 {successCount} 个文件，失败 {failCount} 个文件"; }
        }
        private void lrc2krc(string file)
        {
            try
            {
                byte[] lrcData = File.ReadAllBytes(file);
                if (lrcData.Length == 0)
                {
                    MessageBox.Show("文件是空的！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    failCount++;
                    return;
                }
                byte[] compressedData = ZlibCompress(lrcData);
                byte[] encryptedData = new byte[compressedData.Length];
                for (int i = 0; i < compressedData.Length; i++) { encryptedData[i] = (byte)(compressedData[i] ^ KEYS[i % 16]); }
                byte[] krcHeader = Encoding.ASCII.GetBytes("krc1");
                byte[] krcData = new byte[krcHeader.Length + encryptedData.Length];
                Buffer.BlockCopy(krcHeader, 0, krcData, 0, krcHeader.Length);
                Buffer.BlockCopy(encryptedData, 0, krcData, krcHeader.Length, encryptedData.Length);
                string krcFilePath = Path.ChangeExtension(file, ".krc");
                File.WriteAllBytes(krcFilePath, krcData);
                successCount++;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("错误：没有文件读写权限！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                failCount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"未知错误：{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                failCount++;
            }
        }
        private void krc2lrc(string file)
        {
            try
            {
                byte[] krcData = File.ReadAllBytes(file);
                if (krcData.Length == 0)
                {
                    MessageBox.Show("错误：文件是空的！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    failCount++;
                    return;
                }
                if (krcData.Length < 4 || krcData[0] != 'k' || krcData[1] != 'r' || krcData[2] != 'c' || krcData[3] != '1')
                {
                    MessageBox.Show("错误：不是有效的 KRC 文件（头部标识不符）！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    failCount++;
                    return;
                }
                byte[] encryptedData = new byte[krcData.Length - 4];
                Buffer.BlockCopy(krcData, 4, encryptedData, 0, encryptedData.Length);
                byte[] decodedData = new byte[encryptedData.Length];
                for (int i = 0; i < encryptedData.Length; i++) { decodedData[i] = (byte)(encryptedData[i] ^ KEYS[i % 16]); }
                byte[] lrcData = ZlibDecompress(decodedData);
                string lrcFilePath = Path.ChangeExtension(file, ".lrc");
                File.WriteAllBytes(lrcFilePath, lrcData);
                successCount++;
            }
            catch (InvalidDataException)
            {
                MessageBox.Show("错误：解压失败，可能是文件损坏！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                failCount++;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("错误：没有文件读写权限！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                failCount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"未知错误：{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                failCount++;
            }
        }
        private byte[] ZlibCompress(byte[] data) // Python版Zlib压缩
        {
            using (var memoryStream = new MemoryStream())
            {
                // zlib 头 (CMF + FLG)
                // CMF: 0x78 - 压缩方法 DEFLATE (8), 窗口大小 32K (7)
                // FLG: 0x9C - 压缩级别 6, 无字典, 校验和
                memoryStream.WriteByte(0x78);
                memoryStream.WriteByte(0x9C);
                using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
                { deflateStream.Write(data, 0, data.Length); }
                // zlib 尾部 (ADLER32 校验和)
                uint adler = ComputeAdler32(data);
                memoryStream.WriteByte((byte)(adler >> 24));
                memoryStream.WriteByte((byte)(adler >> 16));
                memoryStream.WriteByte((byte)(adler >> 8));
                memoryStream.WriteByte((byte)adler);
                return memoryStream.ToArray();
            }
        }
        private byte[] ZlibDecompress(byte[] data) // Python版Zlib解压
        {
            // 检查 zlib 头
            if (data.Length < 6 || data[0] != 0x78) { throw new InvalidDataException("无效的 zlib 数据格式"); }
            // 跳过 zlib 头 (2字节)
            int compressedDataLength = data.Length - 6; // 减去头(2)和尾(4)
            byte[] compressedData = new byte[compressedDataLength];
            Buffer.BlockCopy(data, 2, compressedData, 0, compressedDataLength);
            using (var memoryStream = new MemoryStream(compressedData))
            using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                deflateStream.CopyTo(resultStream);
                byte[] decompressedData = resultStream.ToArray();
                uint storedAdler = (uint)((data[data.Length - 4] << 24) |
                                         (data[data.Length - 3] << 16) |
                                         (data[data.Length - 2] << 8) |
                                         data[data.Length - 1]);
                uint computedAdler = ComputeAdler32(decompressedData);

                if (storedAdler != computedAdler) { throw new InvalidDataException("ADLER32 校验和验证失败"); }
                return decompressedData;
            }
        }
        private uint ComputeAdler32(byte[] data) // 计算ADLER32校验和
        {
            uint a = 1, b = 0;
            foreach (byte byteValue in data)
            {
                a = (a + byteValue) % 65521;
                b = (b + a) % 65521;
            }
            return (b << 16) | a;
        }
    }
}