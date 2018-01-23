using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;


namespace XmlOutline.CustomScripts
{
    /// <summary>
    /// Hold the full path and name as an identifier, the xml document and the outline structure
    /// </summary>
    class DocumentModel
    {
        public string FullPath;
        private XDocument xmlDocument;
        public DocumentModel(string path)
        {
            FullPath = path;
	        UpdateXml();
		}
        
        /// <summary>
        /// Updates the XML which can then be used to update the outline
        /// </summary>
        public void UpdateXml()
        {
            var xml = System.IO.File.ReadAllText(FullPath);
            xmlDocument = XDocument.Parse(xml, LoadOptions.None);
            xmlDocument.DescendantNodes().OfType<XComment>().Remove();
        }
    }
}
