using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bib7ae_4
{
    using System.Windows.Media.Effects;

    using Microsoft.Win32;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The radius.
        /// </summary>
        private const int Radius = 13;

        private int strokewidth = 10;

        private Point lastpPoint;

        private List<int> undo;

        private int undoIndex = 0;

        private bool check = true;
        public MainWindow()
        {
            this.InitializeComponent();

            this.undo = new List<int>();
            this.ComboBox.Items.Add("1");
            this.ComboBox.Items.Add("5");
            this.ComboBox.Items.Add("10");
            this.ComboBox.Items.Add("15");
            this.ComboBox.Items.Add("20");
            this.ComboBox.Items.Add("25");
            this.ComboBox.Items.Add("30");
            this.ComboBox.SelectedIndex = 2;

        }

        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.Canvas);
            this.lastpPoint = p;

            if (this.check)
            {
                this.undo.Add(new int());
                this.check = false;
            }


        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Canvas.Children.Clear();
            this.undoIndex = 0;
            this.undo.Clear();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(this.Canvas);

                Line line = new Line();
                this.undo[this.undoIndex]++;
                line.X1 = this.lastpPoint.X;
                line.X2 = p.X;
                line.Y1 = this.lastpPoint.Y;
                line.Y2 = p.Y;
                line.Stroke = new SolidColorBrush(Color.FromArgb((byte)this.Alpha.Value, (byte)this.Rot.Value, (byte)this.Grün.Value, (byte)this.Blau.Value));
                line.StrokeThickness = this.strokewidth;
                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Round;
                this.Canvas.Children.Add(line);
                this.lastpPoint = p;
                
            }

        }

        private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            undoIndex++;
            this.check = true;
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.strokewidth = int.Parse((string)this.ComboBox.SelectedItem);
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.FarbeVorschau.Fill = new SolidColorBrush(Color.FromArgb((byte)this.Alpha.Value, (byte)this.Rot.Value, (byte)this.Grün.Value, (byte)this.Blau.Value));
        }

        private void Undo_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.undo.Count>0)
            {
                this.Canvas.Children.RemoveRange(
                    this.Canvas.Children.Count - this.undo[this.undoIndex - 1],
                    this.undo[this.undoIndex - 1]);
                if (this.undoIndex > 0)
                {
                    this.undo.RemoveAt(this.undoIndex - 1);
                    this.undoIndex--;
                }
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "picture (*.png)|*.png|jpg (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                Rect bounds = VisualTreeHelper.GetDescendantBounds(this.Canvas);
                double dpi = 96d;


                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)bounds.Width,
                    (int)bounds.Height,
                    dpi,
                    dpi,
                    System.Windows.Media.PixelFormats.Default);


                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(this.Canvas);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                try
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    pngEncoder.Save(ms);
                    ms.Close();

                    System.IO.File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Blur_OnClick(object sender, RoutedEventArgs e)
        {

            BlurBitmapEffect blurEffect = new BlurBitmapEffect();
            blurEffect.Radius = this.ShaderStrength.Value;

            blurEffect.KernelType = KernelType.Box;
            foreach (Line canvasChild in this.Canvas.Children)
            {
                canvasChild.BitmapEffect = blurEffect;
            }
        }
    }
}
