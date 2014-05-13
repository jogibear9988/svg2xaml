////////////////////////////////////////////////////////////////////////////////
//
//  SvgImageExtension.cs - This file is part of Svg2Xaml.
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
using System.Windows.Markup;
using System.Resources;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Media;
using System.IO.Compression;

namespace Svg2Xaml
{

  //****************************************************************************
  /// <summary>
  ///   A <see cref="MarkupExtension"/> for loading SVG images.
  /// </summary>
  public class SvgImageExtension
    : MarkupExtension
  {
    //==========================================================================
    private Uri  m_Uri           = null;
    private bool m_IgnoreEffects = false;

    //==========================================================================
    /// <summary>
    ///   Initializes a new <see cref="SvgImageExtension"/> instance.
    /// </summary>
    public SvgImageExtension()
    {
      // ...
    }

    //==========================================================================
    /// <summary>
    ///   Initializes a new <see cref="SvgImageExtension"/> instance.
    /// </summary>
    /// <param name="uri">
    ///   The location of the SVG document.
    /// </param>
    public SvgImageExtension(Uri uri)
    {
      m_Uri = uri;
    }

    //==========================================================================
    /// <summary>
    ///   Overrides <see cref="MarkupExtension.ProvideValue"/> and returns the 
    ///   <see cref="DrawingImage"/> the SVG document is rendered into.
    /// </summary>
    /// <param name="serviceProvider">
    ///   Object that can provide services for the markup extension; 
    ///   <paramref name="serviceProvider"/> is not used.
    /// </param>
    /// <returns>
    ///   The <see cref="DrawingImage"/> the SVG image is rendered into or 
    ///   <c>null</c> in case there has been an error while parsing or 
    ///   rendering.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      try
      {
        using(Stream stream = Application.GetResourceStream(m_Uri).Stream)
          return SvgReader.Load(new GZipStream(stream, System.IO.Compression.CompressionMode.Decompress), new SvgReaderOptions { IgnoreEffects = m_IgnoreEffects });
      }
      catch(Exception exception)
      {
        Debug.WriteLine(exception.GetType() + ": " + exception.Message);
      }

      try
      {
        using(Stream stream = Application.GetResourceStream(m_Uri).Stream)
          return SvgReader.Load(stream, new SvgReaderOptions { IgnoreEffects = m_IgnoreEffects });
      }
      catch(Exception exception) 
      {
        Debug.WriteLine(exception.GetType() + ": " + exception.Message);
        return null;
      }
    }

    //==========================================================================
    /// <summary>
    ///   Gets or sets the location of the SVG image.
    /// </summary>
    public Uri Uri
    {
      get 
      {
        return m_Uri;
      }

      set 
      {
        m_Uri = value;
      }
    }

    //==========================================================================
    /// <summary>
    ///   Gets or sets whether SVG filter effects should be transformed into
    ///   WPF bitmap effects.
    /// </summary>
    public bool IgnoreEffects
    {
      get
      {
        return m_IgnoreEffects;
      }

      set
      {
        m_IgnoreEffects = value;
      }
    }

  } // class SvgImageExtension

}
 