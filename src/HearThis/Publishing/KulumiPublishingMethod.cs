using System;
using System.IO;
using System.Linq;
using SIL.IO;


namespace HearThis.Publishing
{
	/// <summary>
	/// Represents a publishing method specific to Kulumi, extending the SaberPublishingMethod class.
	/// This class provides the necessary properties and methods to publish audio content in the Kulumi format.
	/// </summary>
	public class KulumiPublishingMethod : SaberPublishingMethod
	{

		/// <summary>
		/// Gets the root directory name specific to Kulumi publishing.
		/// This property overrides the base class's RootDirectoryName property to provide a Kulumi-specific directory name.
		/// </summary>
		public override string RootDirectoryName
		{
			get { return "Kulumi"; }
		}
	}
}


/// Note (Created 03/08/2024): For Future Sprints we recommend either adding here or creating a new method for Kulumi Sheep specifically,  
/// and find a way to connect kulumi sheep so they can export data packs directly into kulumi sheep (possibly by detecting what is connected to the computer?)
// that would mean we need to add a drop down UI in the exporting form and have a physical product to connect to test if changing destinations are applicable
// Kulumi sheep requires only 2 levels of navigation, while the method above does three, root folder ("Kulumi"), sub folder ("Book Name"), files (example: "019134Psalm 134")
