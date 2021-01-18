using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Comm100.Framework.Common;
using FileService.Domain.Bo;
using FileService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure.Repositories
{
    public class S3Repository : IS3Repository
    {
        static IAmazonS3 client;
        public S3Repository()
        {
        }

        public async Task Delete(S3SettingsBo settings, string fileLink)
        {
            RegionEndpoint region = RegionEndpoint.GetBySystemName(settings.Region);
            using (client = new AmazonS3Client(settings.APIKey, settings.KeyAccess, region))
            {
                await DeletingAnObject(settings, fileLink);
            }
        }

        public async Task Put(S3SettingsBo settings, S3FileBo file)
        {
            RegionEndpoint region = RegionEndpoint.GetBySystemName(settings.Region);
            using (client = new AmazonS3Client(settings.APIKey, settings.KeyAccess, region))
            {
                await WritingAnObject(settings, file);
            }
        }

        static async Task WritingAnObject(S3SettingsBo settings, S3FileBo file)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    //ContentBody = System.Text.Encoding.UTF8.GetString(file.Content),
                    ContentType = ContentTypeHelper.GetMimeType(file.Name),
                    InputStream = new MemoryStream(file.Content),
                    BucketName = settings.Bucket,
                    Key = file.Link.Replace($"{settings.Address}/", ""),
                    CannedACL = S3CannedACL.PublicRead,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.Name,
                    Inline = ContentTypeHelper.FileIsInline(file.Name)  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                request.Headers.ContentDisposition = cd.ToString();
                request.Headers.CacheControl = "max-age=31536000";
                var result =  await client.PutObjectAsync(request);
                if(result.HttpStatusCode!= System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"An error occurred with the code '{result.HttpStatusCode}' when writing an object");
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception($"An error occurred with the message '{amazonS3Exception.Message}' when writing an object");
                }
            }
        }

        static async Task DeletingAnObject(S3SettingsBo settings, string fileLink)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = settings.Bucket,
                    Key = fileLink.Replace($"{settings.Address}/", ""),
                };

                await client.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception($"An error occurred with the message '{amazonS3Exception.Message}' when deleting an object");
                }
            }
        }

    }
}