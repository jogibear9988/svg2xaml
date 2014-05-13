////////////////////////////////////////////////////////////////////////////////
//
//  SvgScaleTransform.cs - This file is part of Svg2Xaml.
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
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media;

namespace Svg2Xaml
{

  //****************************************************************************
  class SvgScaleTransform
    : SvgTransform
  {
    public readonly double X;
    public readonly double Y;

    //==========================================================================
    public SvgScaleTransform(double x, double y)
    {
      X = x;
      Y = y;
    }

    //==========================================================================
    public SvgScaleTransform(double scale)
    {
      X = scale;
      Y = scale;
    }

    //==========================================================================
    public override Transform ToTransform()
    {
      return new ScaleTransform(X, Y);
    }

    //==========================================================================
    public static new SvgScaleTransform Parse(string transform)
    {
      string[] tokens = transform.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);

      if(tokens.Length == 1)
        return new SvgScaleTransform(Double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat));
      
      if(tokens.Length == 2)
        return new SvgScaleTransform(Double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                     Double.Parse(tokens[1].Trim(), CultureInfo.InvariantCulture.NumberFormat));

      throw new NotSupportedException();
    }

  } // class SvgScaleTransform

}
