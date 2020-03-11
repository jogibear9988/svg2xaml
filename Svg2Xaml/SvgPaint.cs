////////////////////////////////////////////////////////////////////////////////
//
//  SvgPaint.cs - This file is part of Svg2Xaml.
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
  abstract class SvgPaint
  {

    //==========================================================================
    public static SvgPaint Parse(string value)
    {
      if(string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
        throw new ArgumentException("value must not be null or empty", nameof(value));
   
      value = value.Trim();
   
      if(value.StartsWith("url"))
      {
        string url = value.Substring(3).Trim();
        if(url.StartsWith("(") && url.EndsWith(")"))
        {
          url = url.Substring(1, url.Length - 2).Trim();
          if(url.StartsWith("#"))
            return new SvgUrlPaint(url.Substring(1).Trim());
        }
      }

      return new SvgColorPaint(SvgColorParser.Parse(value));
    }

    //==========================================================================
    public abstract Brush ToBrush(SvgBaseElement element);

  } // class SvgPaint

}
