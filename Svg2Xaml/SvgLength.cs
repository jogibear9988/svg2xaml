////////////////////////////////////////////////////////////////////////////////
//
//  SvgLength.cs - This file is part of Svg2Xaml.
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

namespace Svg2Xaml
{

  //****************************************************************************
  class SvgLength
  {

    //==========================================================================
    public readonly double Value;
    public readonly string Unit;

    //==========================================================================
    public SvgLength(double value)
    {
      Value = value;
      Unit = null;
    }


    //==========================================================================
    public SvgLength(double value, string unit)
    {
      Value = value;
      Unit  = unit;
    }

    //==========================================================================
    public static SvgLength Parse(string value)
    {
      if(value == null)
        throw new ArgumentNullException("value");
      value = value.Trim();
      if(value == "")
        throw new ArgumentException("value must not be empty", "value");

      if(value == "inherit")
        return new SvgLength(Double.NaN, null);

      string unit = null;

      foreach(string unit_identifier in new string[] {"in", "cm", "mm", "pt", "pc", "px", "%" })
        if(value.EndsWith(unit_identifier))
        {
          unit  = unit_identifier;
          value = value.Substring(0, value.Length - unit_identifier.Length).Trim();
          break;
        }

      return new SvgLength(Double.Parse(value, CultureInfo.InvariantCulture.NumberFormat), unit);
    }

    //==========================================================================
    public double ToDouble()
    {
      return Value;
    }

  } // class SvgLength

}
