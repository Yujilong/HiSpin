#import 
@interface Clipboard : NSObject
extern "C"
{
    void _copyTextToClipboard(const char *textList);
}
@end