////////////////////////////////////////////////////////////////////////////////
//
//  SvgImageElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace Svg2Xaml
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents an &lt;image&gt; element.
  /// </summary>
  class SvgImageElement
    : SvgDrawableBaseElement
  {
    //==========================================================================
    public readonly SvgCoordinate Y = new SvgCoordinate(0.0);
    public readonly SvgCoordinate X = new SvgCoordinate(0.0);
    public readonly SvgLength Width = new SvgLength(0.0);
    public readonly SvgLength Height = new SvgLength(0.0);

    //==========================================================================
    public readonly string DataType = null;
    public readonly byte[] Data     = null;

    //==========================================================================
    public SvgImageElement(SvgDocument document, SvgBaseElement parent, XElement imageElement)
      : base(document, parent, imageElement)
    {
      XAttribute x_attribute = imageElement.Attribute("x");
      if(x_attribute != null)
        X = SvgCoordinate.Parse(x_attribute.Value);

      XAttribute y_attribute = imageElement.Attribute("y");
      if(y_attribute != null)
        Y = SvgCoordinate.Parse(y_attribute.Value);

      XAttribute width_attribute = imageElement.Attribute("width");
      if(width_attribute != null)
        Width = SvgLength.Parse(width_attribute.Value);

      XAttribute height_attribute = imageElement.Attribute("height");
      if(height_attribute != null)
        Height = SvgLength.Parse(height_attribute.Value);

      XAttribute href_attribute = imageElement.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink"));
      if(href_attribute != null)
      {
        string reference = href_attribute.Value.TrimStart();
        if(reference.StartsWith("data:"))
        {
          reference = reference.Substring(5).TrimStart();
          int index = reference.IndexOf(";");
          if(index > -1)
          {
            string type = reference.Substring(0, index).Trim();
            reference = reference.Substring(index + 1);

            index = reference.IndexOf(",");
            string encoding = reference.Substring(0, index).Trim();
            reference = reference.Substring(index + 1).TrimStart();

            switch(encoding)
            { 
              case "base64":
                Data = Convert.FromBase64String(reference);
                break;

              default:
                throw new NotSupportedException(String.Format("Unsupported encoding: {0}", encoding));
            }

            string[] type_tokens = type.Split('/');
            if(type_tokens.Length != 2)
              throw new NotSupportedException(String.Format("Unsupported type: {0}", type));

            type_tokens[0] = type_tokens[0].Trim();
            if(type_tokens[0] != "image")
              throw new NotSupportedException(String.Format("Unsupported type: {0}", type));

            switch(type_tokens[1].Trim())
            {
              case "jpeg":
                DataType = "jpeg";
                break;

              case "png":
                DataType = "png";
                break;

              case "svg+xml":
                DataType = "svg+xml";
                break;

              default:
                throw new NotSupportedException(String.Format("Unsupported type: {0}", type));
            }
          }
        }
      }
    }

    //==========================================================================
    public override Drawing GetBaseDrawing()
    {
        if (Data == null)
          return null;
        ImageSource imageSource = null;
        switch (DataType)
        {
          case "jpeg":
          case "png":
              var bmp = new BitmapImage();
              bmp.BeginInit();
              bmp.StreamSource = new MemoryStream(Data);
              bmp.EndInit();
              imageSource = bmp;
              break;
          case "svg+xml":
              imageSource = SvgReader.Load(new MemoryStream(Data));
              break;
        }
        if (imageSource == null)
          return null;
        return new ImageDrawing(imageSource, new Rect(
                      new Point(X.ToDouble(), Y.ToDouble()),
                      new Size(Width.ToDouble(), Height.ToDouble())
                      ));
    }

    //==========================================================================
    public override Geometry GetBaseGeometry()
    {
      return new RectangleGeometry(new Rect(
        new Point(X.ToDouble(), Y.ToDouble()),
        new Size(Width.ToDouble(), Height.ToDouble())
        ));
    }

  } // class SvgImageElement

}
