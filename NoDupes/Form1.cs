using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Windows.Forms;

/* 3rd party libs */
using Ganss.Excel;
using MetroFramework.Forms;
using System.Drawing;

namespace NoDupes
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            fillGrid();

            kryptonDataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 234, 234, 234);
            kryptonDataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        public class Product
        {
            [Column("Cod")]
            public string Cod { get; set; }
            [Column("Denumire")]
            public string Denumire { get; set; }
            [Column("UM")]
            public string Unitate { get; set; }
            [Column("Cant.")]
            public int Cant { get; set; }
            [Column("PU")]
            public string Pret { get; set; }
        }

        private void fillGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Cod", typeof(string));
            dt.Columns.Add("Denumire", typeof(string));
            //dt.Columns.Add("UM", typeof(string));
            dt.Columns.Add("Cant", typeof(int));
            dt.Columns.Add("PU", typeof(string));

            var path = Environment.CurrentDirectory;
            var files = Directory.EnumerateFiles(path).Select(p => Path.GetFileName(p))
                .Where(file => new string[] { ".xls", ".xlsx" }.Contains(Path.GetExtension(file)))
                .Select(fn => Path.GetFileName(fn));

            foreach (var file in files)
            {
                var products = new ExcelMapper(file).Fetch<Product>();

                foreach (var product in products)
                {
                    //dt.Rows.Add(product.Cod, product.Denumire, product.Unitate, product.Cant, product.Pret);
                    dt.Rows.Add(product.Cod, product.Denumire, product.Cant, product.Pret);
                }
            }

            var groups = dt.AsEnumerable()
                .GroupBy(x => new {
                    id1 = x.Field<string>("Cod"),
                    id2 = x.Field<string>("Denumire"),
                    //id3 = x.Field<string>("UM"),
                    id3 = x.Field<string>("PU")
                }).ToList();

            DataTable results = dt.Clone();

            foreach (var group in groups)
            {
                //results.Rows.Add(new object[] { group.Key.id1, group.Key.id2, group.Key.id3, group.Sum(x => x.Field<int>("Cant")), group.Key.id4 });
                results.Rows.Add(new object[] { group.Key.id1, group.Key.id2, group.Sum(x => x.Field<int>("Cant")), group.Key.id3 });
            }

            kryptonDataGridView1.DataSource = results;
            kryptonDataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //kryptonDataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            kryptonDataGridView1.Columns[0].Width = 100;
            kryptonDataGridView1.Columns[1].Width = 500;
            kryptonDataGridView1.Columns[2].Width = 50;
            //kryptonDataGridView1.Columns[3].Width = 50;
            kryptonDataGridView1.Columns[3].Width = 100;

            GC.Collect();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            fillGrid();
        }
    }
}
