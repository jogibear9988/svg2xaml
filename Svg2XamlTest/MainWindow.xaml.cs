////////////////////////////////////////////////////////////////////////////////
//
//  MainWindow.xmal.cs - This file is part of Svg2Xaml.
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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Svg2Xaml;

namespace Svg2XamlTest
{

  //****************************************************************************
  public partial class MainWindow 
    : Window
  {
    //==========================================================================
    private readonly string FileName = "test12.svg";

    //==========================================================================
    public MainWindow()
    {
      InitializeComponent();

      using(FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
        //try
        {
          Image.Source = SvgReader.Load(stream);
        }
      /*
        catch(Exception exception)
        {
          TextBlock error_text_block = new TextBlock();
          error_text_block.Text = exception.Message;
          Content = error_text_block;
        }
       * */
    }

  } // class MainWindow

}
