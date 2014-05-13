////////////////////////////////////////////////////////////////////////////////
//
//  SvgPatternElement.cs - This file is part of Svg2Xaml.
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
using System.Xml.Linq;
using System.Windows.Media;
using System;
using System.Diagnostics;

namespace Svg2Xaml
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;pattern&gt; element.
  /// </summary>
  class SvgPatternElement
    : SvgDrawableContainerBaseElement
  {
    //==========================================================================
    public readonly SvgTransform PatternTransform = null;
    public readonly SvgPatternUnits PatternUnits =  SvgPatternUnits.ObjectBoundingBox;
    public readonly SvgLength Width = null;
    public readonly SvgLength Height = null;

    //==========================================================================
    public SvgPatternElement(SvgDocument document, SvgBaseElement parent, XElement patternElement)
      : base(document, parent, patternElement)
    {
      XAttribute pattern_transform_attribute = patternElement.Attribute("patternTransform");
      if(pattern_transform_attribute != null)
        PatternTransform = SvgTransform.Parse(pattern_transform_attribute.Value);

      XAttribute pattern_units_attribute = patternElement.Attribute("patternUnits");
      if(pattern_units_attribute != null)
        switch(pattern_units_attribute.Value)
        {
          case "objectBoundingBox":
            PatternUnits = SvgPatternUnits.ObjectBoundingBox;
            break;

          case "userSpaceOnUse":
            PatternUnits = SvgPatternUnits.UserSpaceOnUse;
            break;

          default:
            throw new NotImplementedException(String.Format("patternUnits value '{0}' is no supported", pattern_units_attribute.Value));
        }

      XAttribute width_attribute = patternElement.Attribute("width");
      if(width_attribute != null)
        Width = SvgLength.Parse(width_attribute.Value);

      XAttribute height_attribute = patternElement.Attribute("height");
      if(height_attribute != null)
        Height = SvgLength.Parse(height_attribute.Value);

    }

    //==========================================================================
    public DrawingBrush ToBrush()
    {
      DrawingBrush brush = null;

      if(Reference == null)
        brush = new DrawingBrush(Draw());
      else
      {
        SvgBaseElement element = Document.Elements[Reference];
        if(element is SvgPatternElement)
          brush = (element as SvgPatternElement).ToBrush();
        else
          throw new NotSupportedException("Other references than patterns are not supported");
      }

      if(brush == null)
        return null;

      if((Width != null) || (Height != null))
      {
        double width = brush.Drawing.Bounds.Width;
        double height = brush.Drawing.Bounds.Height;
      }

      if(PatternUnits == SvgPatternUnits.UserSpaceOnUse)
      {
        brush.ViewportUnits = BrushMappingMode.Absolute;
        brush.Viewport = brush.Drawing.Bounds;
      }

      if(PatternTransform != null)
      {
        DrawingGroup drawing_group = new DrawingGroup();
        drawing_group.Transform = PatternTransform.ToTransform();
        drawing_group.Children.Add(brush.Drawing);
        brush.Drawing = drawing_group;
      }

      return brush;
    }

  } // class SvgPatternElement

}
