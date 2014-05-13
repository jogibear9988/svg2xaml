////////////////////////////////////////////////////////////////////////////////
//
//  MainWindow.xaml.cs - This file is part of Svg2Xaml.
//
//    Copyright (C) 2009,2011 Boris Richter <himself@boris-richter.net>
//
//  --------------------------------------------------------------------------
//
//  Svg2Xaml is free software: you can redistribute it and/or modify it under 
//  the terms of the GNU Lesser General Public License as published by the 
//  Free Software Foundation, either version 3 of the License, or (at your 
//  option) any later version.
//
//  Svg2Xaml is distributed in the hope that it will be useful, but WITHOUT 
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//  FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//  
//  You should have received a copy of the GNU Lesser General Public License 
//  along with Svg2Xaml. If not, see <http://www.gnu.org/licenses/>.
//
//  --------------------------------------------------------------------------
//
//  $LastChangedRevision$
//  $LastChangedDate$
//  $LastChangedBy$
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Svg2Xaml;

namespace Svg2XamlBrowser
{

  //****************************************************************************
  public partial class MainWindow 
    : Window
  {
    //==========================================================================
    private volatile Thread   m_LoadThread    = null;
    private readonly Queue<string> m_LoadFiles = new Queue<string>();

    //==========================================================================
    public MainWindow()
    {
      InitializeComponent();
    }

    //==========================================================================
    private void SetProgress(double progressValue, string text, params object[] args)
    {
      Dispatcher.Invoke((Action)delegate 
      {
        StatusProgressBar.Visibility = Visibility.Visible;
        StatusProgressBar.Value      = progressValue;
        StatusTextBlock.Text         = String.Format(text, args);
      }, 
      DispatcherPriority.Background);
    }

    //==========================================================================
    [STAThread]
    private void LoadFiles(object arg)
    {
      int thumbnail_size = default(int);
      Dispatcher.Invoke((Action)delegate
      {
        thumbnail_size = ThumbnailSize;
      });

      m_LoadThread = Thread.CurrentThread;

      int successful = 0;
      int errorneous = 0;

      while(true)
      {
        string file_name = null;
        int count;
        lock(m_LoadFiles)
        {
          count = m_LoadFiles.Count;
          if(count > 0)
            file_name = m_LoadFiles.Dequeue();
        }
        if(file_name == null)
          break;

        try
        {
          SetProgress(100.0 * DirectoryEntriesListBox.Items.Count / (DirectoryEntriesListBox.Items.Count + count),
                      "Loading {0}...", System.IO.Path.GetFileName(file_name));

          DrawingImage svg_image = null;

          if(file_name.EndsWith("svgz"))
            using(FileStream file_stream = new FileStream(file_name, FileMode.Open, FileAccess.Read))
            using(GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress))
              svg_image = SvgReader.Load(gzip_stream, new SvgReaderOptions(true));
          else
            using(FileStream file_stream = new FileStream(file_name, FileMode.Open, FileAccess.Read))
              svg_image = SvgReader.Load(file_stream, new SvgReaderOptions(true));


          DrawingVisual visual = new DrawingVisual();
          using(DrawingContext drawing_context = visual.RenderOpen())
            drawing_context.DrawRectangle(
              new ImageBrush(svg_image) { Stretch = Stretch.Uniform }, null, new Rect(0, 0, thumbnail_size, thumbnail_size));
          RenderTargetBitmap render_bitmap = new RenderTargetBitmap(thumbnail_size, thumbnail_size, 96, 96, PixelFormats.Pbgra32);
          render_bitmap.Render(visual);
          PngBitmapEncoder encoder = new PngBitmapEncoder();
          encoder.Frames.Add(BitmapFrame.Create(render_bitmap));

          using(MemoryStream stream = new MemoryStream())
          {
            encoder.Save(stream);

            BitmapImage bmp_image = new BitmapImage();
            bmp_image.BeginInit();
            bmp_image.StreamSource = stream;
            bmp_image.CacheOption = BitmapCacheOption.OnLoad;
            bmp_image.EndInit();
            bmp_image.Freeze();

            Dispatcher.Invoke((Action)delegate
            {
              Image image = new Image();
              image.Width = thumbnail_size;
              image.Height = thumbnail_size;
              image.Source = bmp_image;

              ListBoxItem item = new ListBoxItem();
              item.Content = image;
              item.ToolTip = System.IO.Path.GetFileName(file_name);
              item.Tag = file_name;
              item.PreviewMouseDoubleClick += DirectoryEntriesListBox_Item_PreviewMouseDoubleClick;
              DirectoryEntriesListBox.Items.Add(item);
            }, DispatcherPriority.Background);

            ++successful;
          }

        }
        catch(Exception exception)
        {
          Dispatcher.Invoke((Action)delegate
          {
            Border border = new Border();
            border.Width = thumbnail_size;
            border.Height = thumbnail_size;
            border.Background = Brushes.Red;
            border.IsHitTestVisible = false;

            ListBoxItem item = new ListBoxItem();
            item.Content = border;
            item.ToolTip = System.IO.Path.GetFileName(file_name);
            item.Tag = file_name;
            DirectoryEntriesListBox.Items.Add(item);

            JournalListBox.Items.Add(String.Format("{0}: {1}", file_name, exception.Message));

          }, DispatcherPriority.Background);

          ++errorneous;
        }
      }

      Dispatcher.Invoke((Action)delegate
      {

        StatusProgressBar.Visibility = Visibility.Collapsed;
        StatusProgressBar.Value = 0;
        StatusTextBlock.Text = String.Format("Successfully loaded {0} files, {1} files could not be loaded properly", successful, errorneous);

      }, DispatcherPriority.Background);

      m_LoadThread = null;
    }

    //==========================================================================
    void DirectoryEntriesListBox_Item_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ListBoxItem item = sender as ListBoxItem;
      if(item != null)
        new PreviewWindow(item.Tag as string).Show();
    }

    //==========================================================================
    private void UpdateDirectoryTreeView()
    {
      DirectoryTreeView.Items.Clear();

      foreach(string drive in Directory.GetLogicalDrives())
      {
        TreeViewItem drive_item = new TreeViewItem();
        drive_item.Header = drive;
        drive_item.Expanded += DirectoryTreeViewItemExpanded;
        drive_item.Tag = drive;

        DirectoryTreeView.Items.Add(drive_item);

        drive_item.Items.Add("...");
      }
    }

    //==========================================================================
    private string GetFullPath(TreeViewItem directoryItem)
    {
      if(directoryItem.Parent is TreeViewItem)
        return System.IO.Path.Combine(GetFullPath(directoryItem.Parent as TreeViewItem), 
                                      directoryItem.Header as string);
      else
        return directoryItem.Header as string;
    }

    //==========================================================================
    private void UpdateDirectoryTreeViewItem(TreeViewItem directoryItem)
    {
      directoryItem.Items.Clear();

      foreach(string directory in Directory.GetDirectories(GetFullPath(directoryItem)))
      {
        TreeViewItem directory_item = new TreeViewItem();
        directory_item.Header = System.IO.Path.GetFileName( directory);
        directory_item.Tag = System.IO.Path.GetFileName(directory);
        directory_item.Expanded += DirectoryTreeViewItemExpanded;
        directoryItem.Items.Add(directory_item);

        directory_item.Items.Add("...");
      }
    }

    //==========================================================================
    private void DirectoryTreeViewItemExpanded(object sender, RoutedEventArgs e)
    {
      TreeViewItem directory_item = sender as TreeViewItem;
      if(directory_item.Items.Count == 1)
        if(directory_item.Items[0] is string)
          UpdateDirectoryTreeViewItem(directory_item);
    }

    //==========================================================================
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateDirectoryTreeView();
    }

    //==========================================================================
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      // Ensure load thread has been aborted...
      lock(m_LoadFiles)
        m_LoadFiles.Clear();
      while(m_LoadThread != null)
        DispatchOperations();
    }

    //==========================================================================
    private void DispatchOperations()
    {
      DispatcherFrame frame = new DispatcherFrame();
      Dispatcher.BeginInvoke(DispatcherPriority.Background,
          (SendOrPostCallback)delegate(object arg)
          {
            (arg as DispatcherFrame).Continue = false;
          }, frame);
      Dispatcher.PushFrame(frame);
    }

    //==========================================================================
    private void UpdateDirectoryEntriesListBox()
    {
      if (DirectoryTreeView.SelectedItem == null)
      {
        DirectoryEntriesListBox.Items.Clear();
        DirectoryTextBox.Text = null;
      }
      else
      {
        // Ensure load thread has been aborted...
        lock (m_LoadFiles)
          m_LoadFiles.Clear();
        while (m_LoadThread != null)
          DispatchOperations();

        string path = GetFullPath(DirectoryTreeView.SelectedItem as TreeViewItem);

        DirectoryTextBox.Text = path;
        StatusTextBlock.Text = String.Format("Scanning directory {0}...", path);
        StatusProgressBar.Value = 0;
        StatusProgressBar.Visibility = Visibility.Visible;
        JournalListBox.Items.Clear();
        JournalListBox.Items.Add(String.Format("Scanning directory {0}...", path));
        DirectoryEntriesListBox.Items.Clear();

        // Fill queue with the files to load...
        foreach (string directory_entry in Directory.GetFiles(path, "*.svg*"))
          m_LoadFiles.Enqueue(directory_entry);

        // Start load thread and wait until it has been started...
        new Thread(LoadFiles)
        {
          IsBackground   = true,
          Priority       = ThreadPriority.Lowest,
          ApartmentState = ApartmentState.STA
        }.Start(path);
        while (m_LoadThread == null)
          DispatchOperations();

      }
    }

    //==========================================================================
    private void DirectoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      UpdateDirectoryEntriesListBox();
    }

    //==========================================================================
    private void DirectoryTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Enter)
        DirectoryButton_Click(null, null);
    }

    //==========================================================================
    private TreeViewItem ApplyDirectoryInfo(TreeViewItem parent_item, DirectoryInfo directoryInfo)
    {
      if(directoryInfo.Parent == null)
      {
        foreach(TreeViewItem root_item in DirectoryTreeView.Items)
         if(String.Compare((string)root_item.Tag, directoryInfo.Name, true) == 0)
            return root_item;
      }
      else
      {
        TreeViewItem root_item = ApplyDirectoryInfo(null, directoryInfo.Parent);
        if(root_item != null)
        {
          root_item.IsExpanded = true;

          foreach(TreeViewItem child_item in root_item.Items)
            if(String.Compare((string)child_item.Tag, directoryInfo.Name, true) == 0)
              return child_item;
        }
      }

      return null;
    }

    //==========================================================================
    private void DirectoryButton_Click(object sender, RoutedEventArgs e)
    {
      string path = DirectoryTextBox.Text;
      if(path != null)
        try
        {
          TreeViewItem item = ApplyDirectoryInfo(null, new DirectoryInfo(path));
          if(item != null)
            item.IsSelected = true;
        }
        catch(Exception exception)
        {
          MessageBox.Show(exception.Message);
        }
    }

    //==========================================================================                
    /// <summary>
    ///   Gets or sets the value of ThumbnailSize of the MainWindow.
    /// </summary>
    public int ThumbnailSize
    {
      get
      {
        return (int)GetValue(ThumbnailSizeProperty);
      }

      set
      {
        SetValue(ThumbnailSizeProperty, value);
      }
    }

    //==========================================================================
    private static void OnThumbnailSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      (sender as MainWindow).UpdateDirectoryEntriesListBox();
    }

    //==========================================================================
    private static object CoerceThumbnailSize(DependencyObject sender, object value)
    {
      int thumbnail_size = (int)value;
      if(thumbnail_size <= 0)
        return DependencyProperty.UnsetValue;
      return value;
    }

    //==========================================================================
    /// <summary>
    ///   Identifies the <see cref="ThumbnailSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThumbnailSizeProperty =
        DependencyProperty.Register("ThumbnailSize", typeof(int), typeof(MainWindow), new FrameworkPropertyMetadata(64, OnThumbnailSizeChanged, CoerceThumbnailSize));

    //==========================================================================
    private void ThumbnailSizeTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Enter)
        ThumbnailSizeTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
    }



  } // class MainWindow 

}
