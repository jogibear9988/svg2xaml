////////////////////////////////////////////////////////////////////////////////
//
//  SvgColorParser.cs - This file is part of Svg2Xaml.
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
using System.Globalization;
using System.Windows.Media;

namespace Svg2Xaml
{

  //****************************************************************************
  /// <summary>
  ///   Represents an RGB color.
  /// </summary>
  static class SvgColorParser
  {
    //==========================================================================
    public static Color Parse(string value)
    {
      if(string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
        throw new ArgumentException("value must not be null or empty", nameof(value));
   
      value = value.Trim();
      
      if(value.StartsWith("rgb"))
      {
        string color = value.Substring(3).Trim();
        if(color.StartsWith("(") && color.EndsWith(")"))
        {
          color = color.Substring(1, color.Length - 2).Trim();

          string[] components = color.Split(',');
          if(components.Length == 3)
          {
            float r, g, b;

            components[0] = components[0].Trim();
            if(components[0].EndsWith("%"))
            {
              components[0] = components[0].Substring(0, components[0].Length - 1).Trim();
              r = (float)(Double.Parse(components[0], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
            }
            else
              r = (float)(Byte.Parse(components[0]) / 255.0);

            components[1] = components[1].Trim();
            if(components[1].EndsWith("%"))
            {
              components[1] = components[1].Substring(0, components[1].Length - 1).Trim();
              g = (float)(Double.Parse(components[1], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
            }
            else
              g = (float)(Byte.Parse(components[1]) / 255.0);

            components[2] = components[1].Trim();
            if(components[2].EndsWith("%"))
            {
              components[2] = components[2].Substring(0, components[2].Length - 1).Trim();
              b = (float)(Double.Parse(components[2], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
            }
            else
              b = (float)(Byte.Parse(components[2]) / 255.0);

            return Color.FromValues(new[] {r, g, b}, new Uri(String.Empty));
            }
        }
      }

      if(value == "none")
        return Colors.Transparent;

      return (Color) ColorConverter.ConvertFromString(value);
    }

  } // class SvgColor

}
