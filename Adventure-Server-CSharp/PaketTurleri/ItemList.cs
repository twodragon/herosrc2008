﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public enum ItemTypes : ushort
    {
        ITEMTYPE_CONSUMABLEITEM = 151,
        ITEMTYPE_CLASSIFIED = 952,
        ITEMTYPE_MINERALWEAPON2 = 194,
        ITEMTYPE_MINERALARMOR = 963,
        ITEMTYPE_MINERALGEM = 960,
        ITEMTYPE_WTF = 912,
        ITEMTYPE_WTF2 = 911,
        ITEMTYPE_WTF3 = 91,
        ITEMTYPE_MINERALACCESSORY = 196,
        ITEMTYPE_MINERALWEAPON = 962,
        ITEMTYPE_SOCKET = 848,
        ITEMTYPE_MATERIALSTONE = 191,
        ITEMTYPE_MATERIALSTONEGHOSTDAGGER = 190,
        ITEMTYPE_SPECIALABILITY = 188,
        ITEMTYPE_PRODUCEDMATERIAL = 185,
        ITEMTYPE_PRODUCTIONBOOK = 187,
        ITEMTYPE_ANNEXMATERIAL = 193,
        ITEMTYPE_REQUIREDREAGENT = 197,
        ITEMTYPE_COMPOSITIONBOOK = 186,
        ITEMTYPE_EXCHANGEGOLD = 170,
        ITEMTYPE_SPECIALENCHANT = 189,
        ITEMTYPE_GAMBLEBOX = 203,
        ITEMTYPE_PET = 221,
        ITEMTYPE_WICKED = 169,
        ITEMTYPE_DUALWIELD = 100,
        ITEMTYPE_IRONCLUBTHROW = 70,
        ITEMTYPE_INSTURMENTARMS = 99,
        ITEMTYPE_GAUNTLET = 101,
        ITEMTYPE_ONEHANDEDSWORD = 102,
        ITEMTYPE_TWOHANDEDSWORD = 103,
        ITEMTYPE_ROD = 104,
        ITEMTYPE_BOW = 105,
        ITEMTYPE_PETREVIVAL = 223,
        ITEMTYPE_PETFOOD = 222,
        ITEMTYPE_SPECIALMOVEMENT = 152,
        ITEMTYPE_AXE = 107,
        ITEMTYPE_THROWINGWEAPON = 108,
        ITEMTYPE_ARROW = 106,
        ITEMTYPE_MAP = 211,
        ITEMTYPE_MONKSTAFF = 71,
        ITEMTYPE_ANIRONCLUB = 1094,
        ITEMTYPE_CIRCLET = 121,
        ITEMTYPE_MASK = 122,
        ITEMTYPE_BOOTS = 123,
        ITEMTYPE_ARMOR = 124,
        ITEMTYPE_EARRING = 131,
        ITEMTYPE_AMULET = 132,
        ITEMTYPE_BRACELET = 133,
        ITEMTYPE_RING = 134,
        ITEMTYPE_SPECIALRECOVERYHP = 148,
        ITEMTYPE_SPECIALRECOVERYMP = 149,
        ITEMTYPE_SPECIALRECOVERYPETHPMP = 146,
        ITEMTYPE_SPECIALRECOVERYPETMP = 147,
        ITEMTYPE_SPECIALRECOVERYINJURY = 1500,
        ITEMTYPE_SPECIALABILTY10BACKSTRDEXINT = 110,
        ITEMTYPE_SPECIALABILITYALLSTRDEXINT = 112,
        ITEMTYPE_CHARMOFSOUL = 166,
        ITEMTYPE_CHARMOFLUCK = 164,
        ITEMTYPE_SPECIALPROBABILTY = 167,
        ITEMTYPE_SCROLLOFREVIVAL = 163,
        ITEMTYPE_PAIDHUNTINGAREA = 94,
        ITEMTYPE_SUMMON = 93,
        ITEMTYPE_CHANGENAME = 216,
        ITEMTYPE_SPECIALUSAGE = 5458,
        ITEMTYPE_SPECIALRPOBABILITY2ARTISANSHAMMER = 85,
        ITEMTYPE_MODIFICATION = 251,
        ITEMTYPE_SPECIALEXCHANGE = 172,
        ITEMTYPE_CHARMOFLUCKWEAPON = 240,
        ITEMTYPE_CHARMOFLUCKARMOR = 241,
        ITEMTYPE_CHARMOFLUCKACCESSORY = 242,
        ITEMTYPE_TRANSFORMATION = 5550,
        ITEMTYPE_SPECIALSTRENGTHENING = 219,
        ITEMTYPE_SPECIALLEVEL = 165,
        ITEMTYPE_SPECIALPACKAGE = 1192,
        ITEMTYPE_2NDORNAMENT = 602,
        ITEMTYPE_CARRIERPIGEON = 160,
        ITEMTYPE_ATTACKARTSBOOKS = 161,
        ITEMTYPE_SPECIALEXPANSION = 237,
        ITEMTYPE_SPECIALEFFECT = 238,
        ITEMTYPE_PASSIVEARTSBOOK = 162,
        ITEMTYPE_HANHELM = 1145,
        ITEMTYPE_HANMASK = 1146,
        ITEMTYPE_HANARMOR = 1148,
        ITEMTYPE_HANBOOTS = 1147,
        ITEMTYPE_SPECIALSTORE = 5319,
        ITEMTYPE_MASKBUTFACE = 634,
        ITEMTYPE_HAN2NDORNAMENT = 1114,
        ITEMTYPE_SOCKETINITIALIZATION = 73,
        ITEMTYPE_SOCKETHARMONIZERSTABILIZER = 83,
        ITEMTYPE_CHANGEENHANCEOPTIONWEAPON = 81,
        ITEMTYPE_CHANGEENHANCEOPTIONARMOR = 337,
        ITEMTYPE_CHANGEENHANCEOPTIONDECO = 593,
        ITEMTYPE_CHANGENAMEPET = 79,
        ITEMTYPE_PREMIUM = 204,
        ITEMTYPE_PREMIUMFIRESPIRIT = 51,
        ITEMTYPE_PREMIUMWATERSPIRIT = 52,
        ITEMTYPE_CIRCLETPET = 1159,
        ITEMTYPE_ARMORPET = 1160,
        ITEMTYPE_BOOTSPET = 1161,
        ITEMTYPE_HOUSEUNIFORM = 1199,
        ITEMTYPE_QUESTITEM = 202,
        ITEMTYPE_SEALTOOL = 230,
        ITEMTYPE_UNSEALTOOL = 231,
        ITEMTYPE_LATCH = 232,
        ITEMTYPE_COUPON = 254,
        ITEMTYPE_OMOK = 7889,
        ITEMTYPE_DESTROYITEM = 250,
        ITEMTYPE_DRACDUALWIELD = 1124,
        ITEMTYPE_DRACPENTA = 1123,
        ITEMTYPE_DRACSWORD = 1126,
        ITEMTYPE_DRACBLADE = 1127,
        ITEMTYPE_DRACBOW = 1129,
        ITEMTYPE_DRACKNIFE = 1132,
        ITEMTYPE_DRACSPEAR = 1128,
        ITEMTYPE_DRACFIST = 1125,
        ITEMTYPE_DRACAXE = 1131,
        ITEMTYPE_MATERIAL = 225,
        ITEMTYPE_BLACKMAJESTYCIRCLET = 3193,
        ITEMTYPE_BLACKMAJESTYMASK = 3194,
        ITEMTYPE_BLACKMAJESTYARMOR = 3196,
        ITEMTYPE_BLACKMAJESTYBOOTS = 3195,
        ITEMTYPE_BLACKMAJESTYBLADE = 3175,
        ITEMTYPE_FIVEELEMENTSNORMAL = 320,
        ITEMTYPE_FIVEELEMENTSADVANCED = 576,
        ITEMTYPE_FIVEELEMENTSRARE = 832,
        ITEMTYPE_FIVEELEMENTSLEGEND = 1088,
        ITEMTYPE_FIVEELEMENTSULTIMATE = 1344,
        ITEMTYPE_FOODINGREDIENTS = 115,
        ITEMTYPE_MEAL = 115,
        ITEMTYPE_EXTERNALCHANGES = 248,
        ITEMTYPE_SPECIALRECOVERY = 55,
        ITEMTYPE_CELESTIALAUCTION = 57,
    }
    
}