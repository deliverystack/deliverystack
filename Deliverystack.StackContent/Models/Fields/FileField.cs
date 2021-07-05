/*

  "uid": "bltc30002573fcf738a",
  "created_at": "2021-05-17T01:15:29.756Z",
  "updated_at": "2021-05-17T01:15:29.756Z",
  "created_by": "bltf659c3e814f4dce6",
  "updated_by": "bltf659c3e814f4dce6",
  "content_type": "image/jpeg",
  "file_size": "14285",
  "tags": [],
  "filename": "HeadlessArchitectAvatar.jpg",
  "url": "https://images.contentstack.io/v3/assets/blt19bb56b18ed076dc/bltc30002573fcf738a/60a1c3b12474161960b8d9cf/HeadlessArchitectAvatar.jpg",
  "ACL": [],
  "is_dir": false,
  "parent_uid": "blt6151232cd4d5bcbd",
  "_version": 1,
  "title": "HeadlessArchitectAvatar.jpg",
  "dimension": {
    "height": 560,
    "width": 560
  },
  "publish_details": [
    {
      "environment": "blt7542a0a338b928ab",
      "locale": "en-us",
      "time": "2021-05-17T01:20:08.531Z",
      "user": "bltf659c3e814f4dce6",
      "version": 1
    }

*/

namespace Deliverystack.StackContent.Models.Fields
{
    using System.Text.Json.Serialization;

    using Deliverystack.StackContent.Models.Entries;

    public class FileField : ContentstackEntryModelBase
    {
        [JsonPropertyName("file_size")]
        //TODO: wants to use string?
//        public int FileSize { get; set; }
        public string FileSize { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public ImageDimensions Dimension { get; set; }

        [JsonPropertyName("parent_uid")]
        public string ParentUid { get; set; }

        [JsonPropertyName("is_dir")]
        public bool IsFolder { get; set; }

        //"ACL": [],
    }
}