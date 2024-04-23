import os
import shutil
import xml.etree.ElementTree as ET

def remove_needs_translation(xml_file):
    # Parse the XML file
    tree = ET.parse(xml_file)
    root = tree.getroot()

    # Define the namespace and prefix
    ns = {'xliff': 'urn:oasis:names:tc:xliff:document:1.2'}
    ET.register_namespace('', ns['xliff'])  # Register default namespace

    # Find the body element with the registered namespace prefix
    body = root.find('.//xliff:body', ns)

    if body is None:
        print(f"Error: 'body' element not found in {xml_file}")
        return

    # Collect trans-units to remove from the body element
    units_to_remove = []

    for trans_unit in body.findall('.//xliff:trans-unit', ns):
        # Get the target element
        target_elem = trans_unit.find('./xliff:target', ns)
        state_attr = target_elem.get('state')

        # Check if state="needs-translation"
        if state_attr == 'needs-translation':
            units_to_remove.append(trans_unit)

    # Remove trans-unit elements from the body
    for trans_unit in units_to_remove:
        body.remove(trans_unit)

    # Write the modified XML back to file
    tree.write(xml_file, encoding='utf-8', xml_declaration=True)

if __name__ == "__main__":
    folder_path = "Downloaded_HearThis_xlf_files"
    for filename in os.listdir(folder_path):
        if filename.startswith("HearThis.") and filename.endswith(".xlf"):
            xml_file = os.path.join(folder_path, filename)
            print(f"Processing file: {xml_file}")
            remove_needs_translation(xml_file)
