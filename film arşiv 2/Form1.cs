using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace film_arşiv_2
{
    public partial class Form1 : Form
    {
        string baglanti = "Server=localhost;Database=film_arsiv;Uid=root;Pwd=;";
        string yeniAd;
        public Form1()
        {
            InitializeComponent();
        }

        void DgwDoldur()
        {
              
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    string sorgu = "SELECT * FROM filmler;";

                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    dgwFilmler.DataSource = dt;

                   // dgwFilmler.Columns["yonetmen"].Visible = false;
                    //dgwFilmler.Columns["yil"].Visible = false;
                    //dgwFilmler.Columns["film_odul"].Visible = false;

                }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string klasorYolu = @"poster";
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }
            DgwDoldur();
            CmdDoldur();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgwFilmler.SelectedCells.Count > 0)
            {
                string sorgu = "UPDATE filmler SET film_ad=@film_ad, yonetmen = @yonetmen, yil = @yil, tur=@tur, sure= @sure, poster = @poster, imdb_puan = @imdb_puan, film_odul = @film_odul WHERE film_id = @film_id";
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();

                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@film_ad", txtAd.Text);
                    cmd.Parameters.AddWithValue("@yonetmen", txtYonetmen.Text);
                    cmd.Parameters.AddWithValue("@yil", txtYil.Text);
                    cmd.Parameters.AddWithValue("@tur", cmbTur.SelectedValue);
                    cmd.Parameters.AddWithValue("@sure", txtSure.Text);
                    cmd.Parameters.AddWithValue("@imdb_puan", txtPuan.Text);
                    cmd.Parameters.AddWithValue("@film_odul", cbOdul.Checked);
                    cmd.Parameters.AddWithValue("@poster", yeniAd);


                    int film_id = Convert.ToInt32(dgwFilmler.SelectedRows[0].Cells["film_id"].Value);
                    cmd.Parameters.AddWithValue("@film_id", film_id);

                    cmd.ExecuteNonQuery();

                    DgwDoldur();

                }
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "INSERT INTO filmler VALUES(NULL,@film_ad,@yonetmen,@yil,@tur,@sure,@poster,@imdb_puan,@film_odul);";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@film_ad", txtFilmEkle.Text);
                cmd.Parameters.AddWithValue("@yonetmen", txtYonetmenEkle.Text);
                cmd.Parameters.AddWithValue("@yil", txtYeniYil.Text);
                cmd.Parameters.AddWithValue("@tur", cmbTurEkle.SelectedValue);
                cmd.Parameters.AddWithValue("@sure", txtSureEkle.Text);
                cmd.Parameters.AddWithValue("@poster", yeniAd);
                cmd.Parameters.AddWithValue("@imdb_puan", txtYeniPuan.Text);
                cmd.Parameters.AddWithValue("@film_odul", cbOdulEkle.Checked);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Kayıt Eklendi");
                }
            }

            DgwDoldur();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = dgwFilmler.SelectedRows[0];

            int id = Convert.ToInt32(dr.Cells[0].Value);

            string posterYol = Path.Combine(Environment.CurrentDirectory, "poster", dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString());


            DialogResult cevap = MessageBox.Show("Filmi silmek istediğinizden emin misiniz?",
                                                 "Filmi sil", MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);


            if (cevap == DialogResult.Yes)
            {

                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    int film_id = Convert.ToInt32(dgwFilmler.SelectedRows[0].Cells["film_id"].Value);
                    baglan.Open();
                    string sorgu = "DELETE FROM filmler WHERE film_id=@film_id;";
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@film_id", film_id);
                    cmd.ExecuteNonQuery();


                    if (File.Exists(posterYol))
                    {

                        File.Delete(posterYol);
                    }
                    DgwDoldur();
                }
            }
        }
        void CmdDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT DISTINCT tur FROM filmler;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                cmbTur.DataSource = dt;

                cmbTur.DisplayMember = "tur";
                cmbTur.ValueMember = "tur";

                cmbTurEkle.DataSource = dt;

                cmbTurEkle.DisplayMember = "tur";
                cmbTurEkle.ValueMember = "tur";

            }

        }

        private void dgwFilmler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwFilmler.SelectedRows.Count > 0)
            {

                txtAd.Text = dgwFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                txtYonetmen.Text = dgwFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                txtYil.Text = dgwFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                txtSure.Text = dgwFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                txtPuan.Text = dgwFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                cbOdul.Checked = Convert.ToBoolean(dgwFilmler.SelectedRows[0].Cells["film_odul"].Value);

                lblAd.Text = dgwFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                lblYonetmen.Text = dgwFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                lblYil.Text = dgwFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                lblSure.Text = dgwFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                lblPuan.Text = dgwFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                lblTur.Text = dgwFilmler.SelectedRows[0].Cells["tur"].Value.ToString();

                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "poster", dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString());

               pbResim.ImageLocation = null;
                pbResim2.ImageLocation = null;
                pbResimEkle.ImageLocation = null;

                if (File.Exists(dosyaYolu))
                {
                    pbResim.ImageLocation = dosyaYolu;
                    pbResim.SizeMode = PictureBoxSizeMode.StretchImage;

                    pbResim2.ImageLocation = dosyaYolu;
                    pbResim2.SizeMode = PictureBoxSizeMode.StretchImage;

                }

            }
        }

        private void pbResim2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResim.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void pbResimEkle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResimEkle.ImageLocation = null;

            if (File.Exists(hedefDosya))
            {
                pbResimEkle.ImageLocation = hedefDosya;
                pbResimEkle.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void pbResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResim.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
