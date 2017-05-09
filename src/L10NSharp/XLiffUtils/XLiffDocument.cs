﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2009, SIL International. All Rights Reserved.
// <copyright from='2009' to='2009' company='SIL International'>
//		Copyright (c) 2009, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: XLiffDocument.cs
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace L10NSharp.XLiffUtils
{
	#region XLiffDocment class
	/// ----------------------------------------------------------------------------------------
	[XmlRoot("xliff")]
	public class XLiffDocument
	{
        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="XLiffDocument"/> class.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public XLiffDocument()
		{
			//Body = new XLiffBody();
			File = new XLiffFile();
			Version = "2.0";
		}

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the XLIFF version.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("version")]
		public string Version { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlElement("file")]
		public XLiffFile File { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the body.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlElement("body")]
		public XLiffBody Body { get; set; }

		#endregion

		#region Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the translation unit for the specified id.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public TransUnit GetTransUnitForId(string id)
		{
			return File.GetTransUnitForId(id);
		}

		/// ------------------------------------------------------------------------------------
		public IEnumerable<TransUnit> GetTransUnitsForTextInLang(string langId, string text)
		{
			return from tu in File.TransUnits
				   let variant = tu.GetVariantForLang(langId)
				   where variant != null && variant.Value == text
				   select tu;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets all the unique language ids found in the XLIFF file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IEnumerable<string> GetAllVariantLanguagesFound()
		{
			return File.TransUnits.SelectMany(tu => tu.Variants).Select(v => v.Lang).Distinct();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified translation unit.
		/// </summary>
		/// <param name="tu">The translation unit.</param>
		/// <returns>true if the translation unit was successfully added. Otherwise, false.</returns>
		/// ------------------------------------------------------------------------------------
		public bool AddTransUnit(TransUnit tu)
		{
			return File.AddTransUnit(tu);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Remove the specified translation unit.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void RemoveTransUnit(TransUnit tu)
		{
			File.RemoveTransUnit(tu);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Saves the XLIFFDocument to the specified XLIFF file.
        /// </summary>
        /// ------------------------------------------------------------------------------------
        public void Save(string xliffFile)
		{
            XLiffXmlSerializationHelper.SerializeToFile(xliffFile, this);
		}

        /// ------------------------------------------------------------------------------------
        /// <summary>
        /// Reads the specified XLIFF file and returns a XLIFFDocument containing the information
        /// in the file.
        /// </summary>
        /// <param name= "xLiffFile">The XLiff file to read.</param>
        /// ------------------------------------------------------------------------------------
        public static XLiffDocument Read(string xLiffFile)
		{
			if (!System.IO.File.Exists(xLiffFile))
				throw new FileNotFoundException("XLiff file not found.", xLiffFile);

			Exception e;
			var xLiffDoc = XLiffXmlSerializationHelper.DeserializeFromFile<XLiffDocument>(xLiffFile, out e);

			if (e != null)
				throw e;

			return xLiffDoc;
		}

		#endregion

		/// <summary>
		/// When we change ids after people have already been localizing, we have a BIG PROBLEM.
		/// This helps with the common case were we just changed the hierarchical organizaiton of the id,
		/// that is, the parts of the id before th final '.'.
		/// </summary>
		/// <param name="id"></param>
		 public TransUnit GetTransUnitForOrphan(TransUnit orphan)
		{
			 return File.GetTransUnitForOrphan(orphan);
		}
	}

	#endregion
}

