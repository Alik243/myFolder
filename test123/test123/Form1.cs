using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace test123
{
    public partial class Form1 : Form
    {
        byte[] abc;
        byte[,] table;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Открываем файл
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.Filter = "Image files(*.WAV;*.JPG;*.MP3)|*.WAV;*.JPG;*.MP3|All Files(*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pb1.Image = new Bitmap(openFile.FileName);
                    tbSelect.Text = openFile.FileName;
                }
                catch (Exception)
                {
                    tbSelect.Text = openFile.FileName;
                    MessageBox.Show("Ошибка открытия картинки", " ", MessageBoxButtons.OK);
                }
            }
        }

        // Выбрать что делать
        private void rb1_CheckedChanged(object sender, EventArgs e)
        {
            if (rb1.Checked)
            {
                rb2.Checked = false;
            }
        }
        private void rb2_CheckedChanged(object sender, EventArgs e)
        {
            if (rb2.Checked)
            {
                rb1.Checked = false;
            }
        }

        // Начальные значения при запуске
        private void Form1_Load(object sender, EventArgs e)
        {
            rb1.Checked = true;

            abc = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                abc[i] = Convert.ToByte(i);
            }

            table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    table[i, j] = abc[(i + j) % 256];
                }
            }
        }

        // Начало шифрования
        private void btnGo_Click(object sender, EventArgs e)
        {
            if (!File.Exists(tbSelect.Text))
            {
                MessageBox.Show("Файл не существует.");
                return;
            }
            if (String.IsNullOrEmpty(tbKey.Text))
            {
                MessageBox.Show("Введите ключ для шифрования");
                return;
            }


            try
            {
                byte[] fileContent = File.ReadAllBytes(tbSelect.Text);
                byte[] passwortTmp = Encoding.ASCII.GetBytes(tbKey.Text);
                byte[] keys = new byte[fileContent.Length];
                for (int i = 0; i < fileContent.Length; i++)
                {
                    keys[i] = passwortTmp[i % passwortTmp.Length];
                }

                byte[] result = new byte[fileContent.Length];

                //Encrypt

                if (rb1.Checked)
                {
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; j < 256; j++)
                        {
                            if (abc[j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }
                        for (int j = 0; j < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }
                        result[i] = table[keyIndex, valueIndex];
                    }

                }

                // Decrypt
                else
                {
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        
                        for (int j = 0; j < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }
                        for (int j = 0; j < 256; j++)
                        {
                            if (table[keyIndex,j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }
                        result[i] = abc[valueIndex];
                    }
                }

                //save result
                String fileExt = Path.GetExtension(tbSelect.Text);
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Files(*" + fileExt + ") | *" + fileExt;
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sd.FileName, result);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Файл занят");
                return;
            }


        }
    }
}
