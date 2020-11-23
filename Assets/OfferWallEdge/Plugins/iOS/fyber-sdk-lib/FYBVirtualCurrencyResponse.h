//
//
// Copyright (c) 2017 Fyber. All rights reserved.
//
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

/**
 *  FYBVirtualCurrencyResponse is the object that is received when requesting the delta of coins to the Virtual Currency Server
 *
 *  @see FYBVirtualCurrencyClient, FYBVirtualCurrencyClientDelegate
 */
@interface FYBVirtualCurrencyResponse : NSObject

/**
 *  Latest transaction ID for your user and app IDs, as reported by the server. It is used to keep track of new transactions between invocations to requestDeltaOfCoins
 */
@property (nonatomic, copy) NSString *latestTransactionId;

/**
 *  The ID of the currency being earned by the user
 */
@property (nonatomic, copy) NSString *currencyId;

/**
 *  The name of the currency being earned by the user
 */
@property (nonatomic, copy) NSString *currencyName;

/**
 *  Amount of coins earned by the user
 */
@property (nonatomic, assign) CGFloat deltaOfCoins;

/**
 *  YES if the currency is marked as default in the Fyber Dashboard. NO otherwise
 */
@property (nonatomic, assign) BOOL isDefault;

@end