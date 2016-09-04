The idea for this experimental project is to see whether it makes sense to store SRTM tiles as PNGs
on a cloud storage (Amazon S3 bucket) so we can avoid problems with NASA authentication system (OAuth 2). 
Additionally, the storage would offer a multi-level access to SRTM data.

The general idea of what is needed:
1. A tool that downloads all of the SRTM R1 tiles from the NASA server (we have to figure out how
to circumvent OAuth - probably by directly inserting any needed cookies into the HTTP requests).
The tool should support continuation - if a certain tile is already in the local storage, do not
download it from the server.
2. A tool that converts HGTs into PNGs. It should also support continuation.
3. A tool that produces a higher-level SRTM tiles (by merging 4 tiles into one and rescaling the
data by 50%). It should also support continuation.
4. A tool that uploads the contents of the local storage to the cloud. It should also support 
continuation.

