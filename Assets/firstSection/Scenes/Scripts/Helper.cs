

public class Helper
{ 
    public const string baseUrl = "https://greedyverse.co/";
    public const string baseUrl2 = "https://greedyverseblockchainoperation.herokuapp.com/";
    public const string createWalletUrl = baseUrl2 + "create-seedPhrase";
    public const string validateSeedPhraseUrl = baseUrl2 + "is-valid-seed-phrase?mnemonic=";
    public const string importWalletUrl = baseUrl2 + "create-wallet?memornic_phrase=";
    public const string importWalletWithPrivateKeyUrl = baseUrl2 + "create-wallet-with-privateKey?privateKey=";
    public const string loginUrl = baseUrl + "gameData/authenticator/login.php";
    public const string signupUrl = baseUrl + "gameData/authenticator/signup.php";
    public const string send_verification_email = baseUrl + "gameData/authenticator/send-verification-email.php";
    public const string verify_email = baseUrl + "gameData/authenticator/verify-email.php";
    public const string versionNo = "v.1.0.0";

    public const string userWalletDataKey = "ckxkj23393.23_+*2311232SLMX023cm24o8ng48669";
    public const string userDetailsDataKey = "0xc24_*12_939_027764_727263Lajzb72826454929";
    public const string gameContractAddress = "0x23EEb9971B4A58BFe89521ca99a9bB7270A96E97";
    public static string chainId = "4000";      // MainNet:56 TestNet:97 Offchain:4000
    public const string refineryURL = "https://refinable.com/";
    public const string battleMapKey = "23393.23_+*2311232Sbatlekalckcm24o8ng48669jLMX023kx";
    public static string connection_instance_id = "";
    public static bool logedInFromSignUp = false;



    public static string getUser1155NFT(string walletAddress,string chainId,string tokenId, string sessionID)
    {
        return $"{baseUrl}gameData/mechanics/getNftsdetails/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}&id={tokenId}";
    }

    public static string getNftAmountLeftAndPrice(string walletAddress, string chainId, string tokenId, string sessionID)
    {
        return $"{baseUrl}gameData/mechanics/getNft-left-amount-and-price/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}&id={tokenId}";
    }

    public static string mintNft(string walletAddress, string chainId, string tokenId, string sessionID, string cost, string amount, string privateKey)
    {
        return $"{baseUrl}gameData/mechanics/nftMint?sessionID={sessionID}&chainID={chainId}&privateKey={privateKey}&cost={cost}&nftID={tokenId}&amount={amount}&address={walletAddress}";
    }

    public static string getBNBbalance(string walletAddress, string chainId, string sessionID)
    {
        return $"{baseUrl}gameData/mechanics/get-bnb-balance?sessionID={sessionID}&chainID={chainId}&address={walletAddress}";
    }

    public static string get_nft_details_and_used_amount(string walletAddress, string chainId, string tokenId, string sessionID)
    {
        return $"{baseUrl}gameData/mechanics/get-nft-details-and-used-amount/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}&id={tokenId}";
    }

    public static string train(string walletAddress, string chainId, string tokenId, string sessionID, string MapNo)
    {
        return $"{baseUrl}gameData/mechanics/train-warrior/index.php?address={walletAddress}&sessionID={sessionID}&chainID={chainId}&nftID={tokenId}&MapNo={MapNo}";
    }

    public static string add_to_map(string jCol, string iRow, string mapItemPosx, string mapItemPosy, string walletAddress, string chainId, string tokenId, string sessionID, string MapNo, string structureIndex, string grassType, string strucIDinCgame, string cloneName)
    {
        return $"{baseUrl}gameData/mechanics/RegisterNewMapItem/add-item-to-map.php?iRow={iRow}&jCol={jCol}&mapItemPosx={mapItemPosx}&mapItemPosy={mapItemPosy}&address={walletAddress}&sessionID={sessionID}&chainID={chainId}&nftID={tokenId}&MapNo={MapNo}&structureIndex={structureIndex}&grassType={grassType}&strucIDinCgame={strucIDinCgame}&cloneName={cloneName}";
    }

    public static string change_item_on_map(string mapItemPosx, string mapItemPosy, string walletAddress, string chainId, string tokenId, string sessionID, string MapNo, string newmapItemPosx, string newmapItemPosy)
    {
        return $"{baseUrl}gameData/mechanics/changeItemPosition/index.php?mapItemPosx={mapItemPosx}&mapItemPosy={mapItemPosy}&address={walletAddress}&sessionID={sessionID}&MapNo={MapNo}&nftID={tokenId}&chainID={chainId}&newmapItemPosx={newmapItemPosx}&newmapItemPosy={newmapItemPosy}";
    }

    public static string remove_item_from_map(string mapItemPosx, string mapItemPosy, string walletAddress, string chainId, string tokenId, string sessionID, string MapNo)
    {
        return $"{baseUrl}gameData/mechanics/removeItem/index.php?mapItemPosx={mapItemPosx}&mapItemPosy={mapItemPosy}&address={walletAddress}&sessionID={sessionID}&MapNo={MapNo}&nftID={tokenId}&chainID={chainId}";
    }

    public static string getHealthAliveOrDead(string walletAddress, string chainId, string tokenId, string sessionID)
    {
        return $"{baseUrl}gameData/mechanics/get-health-alive-dead-nfts/index.php?sessionID={sessionID}&chainID={chainId}&address={walletAddress}&nftID={tokenId}";
    }
    public static string revive(string privateKey, string chainId, string address, string tokenId, string sessionID, string amount, string cost)
    {
        return $"{baseUrl}gameData/mechanics/revive?address={address}&sessionID={sessionID}&chainId={chainId}&privateKey={privateKey}&cost={cost}&nftID={tokenId}&amount={amount}";
    }
    public static string constructionSpeedUp(string mapItemPosx, string mapItemPosy, string walletAddress, string chainId, string tokenId, string sessionID, string MapNo, string privateKey)
    {
        return $"{baseUrl}gameData/mechanics/building-speedup/index.php?x_pos={mapItemPosx}&y_pos={mapItemPosy}&address={walletAddress}&sessionID={sessionID}&chainID={chainId}&NftID={tokenId}&MapNo={MapNo}&privateKey={privateKey}";
    }

    public static string constructionSpeedUpCost(string chainId, string sessionID, string walletAddress)
    {
        return $"{baseUrl}gameData/mechanics/get-construction-speedup-cost/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}";
    }

    public static string loadMap(string walletAddress, string mapNo)
    {
        if(int.Parse(chainId) == 4000)
        {
            return $"{baseUrl}gameData/offchain_mechanics/mapverficatorLayer2/maps/{walletAddress}_{mapNo}.txt";
        }
        else
        {
            return $"{baseUrl}gameData/mechanics/mapverficatorLayer2/maps/{walletAddress}_{mapNo}.txt";
        }
        
    }

    public static string trainingSpeedUp(string walletAddress, string chainId, string sessionID, string MapNo, string privateKey, string speedup_items)
    {
        return $"{baseUrl}gameData/mechanics/training-speedup/index.php?address={walletAddress}&sessionID={sessionID}&chainID={chainId}&MapNo={MapNo}&privateKey={privateKey}&speedup_items={speedup_items}";
    }
    public static string trainingSpeedUpCost(string chainId, string sessionID, string walletAddress)
    {
        return $"{baseUrl}gameData/mechanics/get-training-speedup-cost/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}";
    }

    public static string getBalances(string sessionID, string walletAddress, string chainId, string MapNo)
    {
        return $"{baseUrl}gameData/mechanics/get-trainedTroops-bnbBalance-gverseBalance/index.php?sessionID={sessionID}&address={walletAddress}&chainID={chainId}&MapNo={MapNo}";
    }
    public static string confirmPassword(string walletAddress, string password)
    {
        return $"{baseUrl}gameData/authenticator/confirmUser.php?address={walletAddress}&password={password}";
    }


    public static string getUnclaimedToken(string sessionID, string walletAddress, string chainId)
    {
        return $"{baseUrl}gameData/mechanics/get-unclaimed-tokens/index.php?sessionID={sessionID}&address={walletAddress}&chainID={chainId}";
    }

    public static string logOut(string walletAddress)
    {
        return $"{baseUrl}gameData/authenticator/logout.php?address={walletAddress}";
    }
    public static string claimTokens(string sessionID, string walletAddress, string chainId, string privateKey)
    {
        return $"{baseUrl}gameData/mechanics/claim-tokens/index.php?address={walletAddress}&sessionID={sessionID}&chainID={chainId}&privateKey={privateKey}";
    }
    public static string notifications(string sessionID, string walletAddress)
    {
        return $"{baseUrl}gameData/mechanics/notifications/index.php?address={walletAddress}&sessionID={sessionID}";
    }
    public static string findAndMatchPlayer(string sessionID, string walletAddress, string chainId, string mapNo)
    {
        return $"{baseUrl}gameData/mechanics/find-and-match-player/index.php?address={walletAddress}&sessionID={sessionID}&chainID={chainId}&MapNo={mapNo}";
    }
    public static string getWether(string sessionID, string walletAddress)
    {
        return $"{baseUrl}gameData/mechanics/getWether/index.php?address={walletAddress}&sessionID={sessionID}";
    }
    public static string getInstanceMapData(string instanceID)
    {
        if (int.Parse(chainId) == 4000)
        {
            return $"{baseUrl}gameData/offchain_mechanics/get-instance-mapData/index.php?instanceID={instanceID}";
        }
        else
        {
            return $"{baseUrl}gameData/mechanics/get-instance-mapData/index.php?instanceID={instanceID}";
        }
    }
    public static string get_player_army(string walletAddress, string mapNo, string sessionID, string chainId)
    {
        return $"{baseUrl}gameData/mechanics/get-player-army/index.php?address={walletAddress}&sessionID={sessionID}&mapNo={mapNo}&chainId={chainId}";
    }
    public static string check_new_version(string versionNo)
    {
        return $"{baseUrl}gameData/generalChecks/check-app-new-version/index.php?versionNo={versionNo}";
    }
    public static string changeUsername(string walletAddress, string sessionID, string chainId, string username)
    {
        return $"{baseUrl}gameData/mechanics/changeUsername/index.php?address={walletAddress}&sessionID={sessionID}&username={username}&chainId={chainId}";
    }
    public static string getProfileDetails(string walletAddress, string sessionID, string chainId)
    {
        return $"{baseUrl}gameData/mechanics/get-profile-details/index.php?address={walletAddress}&sessionID={sessionID}&chainId={chainId}";
    }
}
