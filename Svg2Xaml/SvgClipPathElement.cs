////////////////////////////////////////////////////////////////////////////////
//
//  SvgClipPathElement.cs - This file is part of Svg2Xaml.
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

namespace Svg2Xaml
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;clipPath&gt; element.
  /// </summary>
  class SvgClipPathElement
    : SvgContainerBaseElement
  {
    //==========================================================================
    public SvgClipPathElement(SvgDocument document, SvgBaseElement parent, XElement clipPathElement)
      : base(document, parent, clipPathElement)
    {
      // ...
    }

    //==========================================================================
    public Geometry GetClipGeometry()
    {
      GeometryGroup geometry_group = new GeometryGroup();

      foreach(SvgBaseElement child_element in Children)
      {
        SvgBaseElement element = child_element;
        if(element is SvgUseElement)
          element = (element as SvgUseElement).GetElement();


        if(element is SvgDrawableBaseElement)
        {
          Geometry geometry = (element as SvgDrawableBaseElement).GetGeometry();
          if(geometry != null)
            geometry_group.Children.Add(geometry);
        }
        else if(element is SvgDrawableContainerBaseElement)
        {
          Geometry geometry = (element as SvgDrawableContainerBaseElement).GetGeometry();
          if(geometry != null)
            geometry_group.Children.Add(geometry);
        }
      }

      return geometry_group;
    }

  } // class SvgClipPathElement

}
