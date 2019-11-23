using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FileService.Domain.Bo;
using FileService.Domain.Interfaces;

namespace FileService.Infrastructure.Repositories
{
    public class S3Repository : IS3Repository
    {
        //static string bucketName = "donybucket6";
        //static string keyName = "AKIAWZX4KNG5Z2P4MRWB";
        static IAmazonS3 client;
        public void Delete(S3SettingsBo settings, string fileKey)
        {
            throw new NotImplementedException();
        }

        public void Put(S3SettingsBo settings, S3FileBo file)
        {
            using (client = new AmazonS3Client(settings.APIKey, "0AqiQCaMTwq3LLQ1g2SQmn17ssKIO5nrB9U4SMc8", RegionEndpoint.USWest2))
            {
                WritingAnObject(settings, file);
            }
        }


        static void WritingAnObject(S3SettingsBo settings, S3FileBo file)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    ContentBody = System.Text.Encoding.UTF8.GetString(file.Content),
                    BucketName = settings.Bucket,
                    Key =  file.Name,
                };
                Task<PutObjectResponse> response = client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
        }
    }
}