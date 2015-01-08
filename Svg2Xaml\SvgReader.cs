//==========================================================================
    /// <summary>
    ///   Loads an SVG document and renders it into a 
    ///   <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="uri">
    ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
    ///   document.
    /// </param>
    /// <param name="options">
    ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
    ///   rendering the SVG document.
    /// </param>
    /// <returns>
    ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
    /// </returns>
    public static DrawingImage Load(string uri, SvgReaderOptions options)
    {
        if (options == null)
            options = new SvgReaderOptions();

        XDocument document = XDocument.Load(uri);
        if (document.Root.Name.NamespaceName != "http://www.w3.org/2000/svg")
            throw new XmlException("Root element is not in namespace 'http://www.w3.org/2000/svg'.");
        if (document.Root.Name.LocalName != "svg")
            throw new XmlException("Root element is not an <svg> element.");

        return new SvgDocument(document.Root, options).Draw();
    }
