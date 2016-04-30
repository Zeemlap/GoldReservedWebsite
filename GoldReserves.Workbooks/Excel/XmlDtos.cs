using System;
using System.Xml.Serialization;

namespace GoldReserves.Workbooks.Excel
{

    public static class XmlConstants
    {
        public const string Namespace_SpreadsheetMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        public const string Namespace_Drawing_Main = "http://schemas.openxmlformats.org/drawingml/2006/main";
        public const string Namespace_Drawing_Spreadsheet = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
        public const string Namespace_Relationships = "http://schemas.openxmlformats.org/package/2006/relationships";
        public const string Namespace_OfficeRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        public const string Namespace_OfficeRelationships_Drawing = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";

    }

    public class CellXml
    {
        [XmlAttribute("r")]
        public string r;
        [XmlAttribute("t")]
        public string t;
        /// <summary>
        /// Original value of the Excel cell
        /// </summary>
        [XmlElement("v")]
        public string v;
    }

    [Serializable]
    [XmlType(Namespace = XmlConstants.Namespace_Relationships)]
    [XmlRoot("Relationships", Namespace = XmlConstants.Namespace_Relationships)]
    public class RelationshipRootXml
    {
        [XmlElement("Relationship")]
        public RelationshipXml[] Relationships;
    }
    public class RelationshipXml
    {
        [XmlAttribute("Id")]
        public string Id;
        [XmlAttribute("Type")]
        public string Type;
        [XmlAttribute("Target")]
        public string Target;
    }


    [Serializable]
    [XmlType(Namespace = XmlConstants.Namespace_SpreadsheetMain)]
    [XmlRoot("sst", Namespace = XmlConstants.Namespace_SpreadsheetMain)]
    public class SharedStringTableXml
    {
        [XmlAttribute("uniqueCount")]
        public string UniqueCount;
        [XmlAttribute("count")]
        public string Count;
        [XmlElement("si")]
        public SharedStringXml[] si;

        public SharedStringTableXml()
        {
        }
    }

    public class SharedStringXml
    {
        public string t;

        [XmlElement("r")]
        public DrawingRXml1[] r;
    }
    public class DrawingRXml1
    {
        public string t;
        public RPrXml1 rPr;

        [XmlElement("r")]
        public DrawingRXml1[] r;
    }
    public class RPrXml1
    {
        public RPrVertAlignXml1 vertAlign;
    }
    public class RPrVertAlignXml1
    {
        public string val;
    }

    [Serializable()]
    [XmlType(Namespace = XmlConstants.Namespace_SpreadsheetMain)]
    [XmlRoot("worksheet", Namespace = XmlConstants.Namespace_SpreadsheetMain)]
    public class WorksheetXml
    {
        [XmlArray("sheetData")]
        [XmlArrayItem("row")]
        public RowXml[] Rows;

        [XmlElement("drawing")]
        public WorksheetDrawingRefXml[] Drawings;

        public WorksheetXml()
        {
        }
    }

    public class WorksheetDrawingRefXml
    {
        [XmlAttribute("id", Namespace = XmlConstants.Namespace_OfficeRelationships)]
        public string id;
    }

    [Serializable]
    [XmlType(Namespace = XmlConstants.Namespace_OfficeRelationships_Drawing)]
    [XmlRoot("wsDr", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
    public class DrawingXml
    {
        [XmlElement("twoCellAnchor", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public DrawingTwoCellAnchorXml TwoCellAnchor;

    }

    public class DrawingTwoCellAnchorXml
    {
        [XmlElement("from", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public DrawingAnchorPoint from;
        [XmlElement("to", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public DrawingAnchorPoint to;
        [XmlElement("sp", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public DrawingSpXml sp;
    }

    public class DrawingAnchorPoint
    {
        [XmlElement("col", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public int col;
        [XmlElement("colOff", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public int colOff;
        [XmlElement("row", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public int row;
        [XmlElement("rowOff", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public int rowOff;
    }

    public class DrawingSpXml
    {
        [XmlElement("txBody", Namespace = XmlConstants.Namespace_Drawing_Spreadsheet)]
        public DrawingSpTxBodyXml txBody;
    }

    public class DrawingSpTxBodyXml
    {
        [XmlElement("p", Namespace = XmlConstants.Namespace_Drawing_Main)]
        public DrawingPXml[] p;
    }

    public class DrawingPXml
    {
        [XmlElement("r", Namespace = XmlConstants.Namespace_Drawing_Main)]
        public DrawingRXml2[] r; 
    }

    public class DrawingRXml2
    {
        [XmlElement("t", Namespace = XmlConstants.Namespace_Drawing_Main)]
        public string t;
    }
    public class RowXml
    {
        [XmlElement("c")]
        public CellXml[] c;
    }


    
}
