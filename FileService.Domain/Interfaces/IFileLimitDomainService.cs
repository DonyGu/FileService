using FileService.Domain.Bo;
using FileService.Domain.Entities;

namespace FileService.Domain.Interfaces
{
    public interface IFileLimitDomainService
    {
        // test file size
        // test name in black list
        // test if exe by file content 
        // if it is a zip file, unzip first
        // throw FileInBlackListException when test not pass
        void Check(CheckFileLimitBo bo);
    }
}