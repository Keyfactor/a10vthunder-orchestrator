@echo off

cd C:\Users\bhill\source\repos\a10vthunder-orchestrator\A10vThunderTestConsole\bin\Debug\net8.0
set ClientMachine=
set A10ApiUser=admin
set A10ApiPassword=
set ScpUserName=ec2-user
set ScpPassword=
set ScpPort=22
set OrchToScpServerIp=
set A10ToScpServerIp=

set clientmachine=%ClientMachine%
set password=%A10ApiPassword%
set user=%A10ApiUser%
set storepath=shared



echo ***********************************
echo Starting Ssl StoreType Test Cases
echo ***********************************


echo ***********************************
echo Starting Management Test Cases
echo ***********************************
set casename=Management


set cert=%random%
set casename=Management
set mgt=add
set overwrite=false


echo ************************************************************************************************************************
echo TC1 %mgt%.  Should do the %mgt% and add anything in the chain
echo ************************************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=remove
set overwrite=false

echo:
echo *******************************************************************************************************
echo TC2 %mgt% unbound Cert.  Should %mgt% the cert since there are no dependencies
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=add
set overwrite=false
set storepath=keyfactor2

echo:
echo *******************************************************************************************************
echo TC3 %mgt% unbound Cert to New Different Partition.  Should %mgt% the cert to Partition
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=remove

echo:
echo *******************************************************************************************************
echo TC4 %mgt% unbound Cert in Different Partition.  Should %mgt% the cert from Partition
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=add
set overwrite=false
set storepath=shared
set cert=%random%

echo:
echo *******************************************************************************************************
echo TC5 Setup Unbound Renew Scenario Unbound Certificate
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set overwrite=false
set storepath=shared

echo:
echo *******************************************************************************************************
echo TC5a Unbound Renew certificate with no overwrite, should warn user that overwrite flag is required
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=add
set storepath=shared
set overwrite=true

echo:
echo *******************************************************************************************************
echo TC5b Unbound Renew certificate with overwrite, should overwrite since flag is used
echo *******************************************************************************************************
echo overwrite: %overwrite%
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl



set mgt=add
set storepath=shared
set overwrite=true

echo:
echo *******************************************************************************************************
echo TC5c Bound Renew certificate with overwrite, should re-name cert and rebind it to all locations
echo *******************************************************************************************************
echo overwrite: %overwrite%
set /p cert=Please enter bound cert name:
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

set mgt=add
set storepath=keyfactor2
set overwrite=true

echo:
echo ******************************************************************************************************************
echo TC6 Bound Renew Different Partition certificate with overwrite, should re-name cert and rebind it to all locations
echo ******************************************************************************************************************
echo overwrite: %overwrite%
set /p cert=Please enter bound cert name:
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl


set mgt=remove
set storepath=keyfactor2
set overwrite=true

echo:
echo ******************************************************************************************************************
echo TC7 Remove Bound certificate - This should not be allowed and should error out
echo ******************************************************************************************************************
echo overwrite: %overwrite%
set /p cert=Please enter bound cert name:
echo cert name: %cert%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl


set casename=Inventory
set storepath=keyfactor2


echo:
echo ******************************************************************************************************************
echo TC8 Inventory from a Partition 
echo ******************************************************************************************************************
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

:TC8

set casename=Inventory
set storepath=shared


echo:
echo ******************************************************************************************************************
echo TC9 Inventory from  shared location 
echo ******************************************************************************************************************
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=ssl

echo ***********************************
echo Starting mgmt StoreType Test Cases
echo ***********************************

:TC10

echo ***********************************
echo Starting Management Test Cases
echo ***********************************
set casename=Management

set mgt=add
set storepath=/home/ec2-user
set overwrite=true
set cert=%random%

echo:
echo ******************************************************************************************************************
echo TC10 Add New Certificate to Location and Bind to Management Port
echo ******************************************************************************************************************
echo overwrite: %overwrite%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=mgmt -orchtoscpserverip=%OrchToScpServerIp% -a10toscpserverip=%A10ToScpServerIp% -scpusername=%ScpUserName% -scppassword=%ScpPassword% -scpport=%ScpPort%


echo:
echo ******************************************************************************************************************
echo TC11 Renew certificate bound to Management Port should rep and bind to mgmt port
echo ******************************************************************************************************************
echo overwrite: %overwrite%
echo storepath: %storepath%

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=mgmt -orchtoscpserverip=%OrchToScpServerIp% -a10toscpserverip=%A10ToScpServerIp% -scpusername=%ScpUserName% -scppassword=%ScpPassword% -scpport=%ScpPort%


echo:
echo ******************************************************************************************************************
echo TC12 Renew/Repl certificate bound to Management Port with out overwrite, should fail and warn user
echo ******************************************************************************************************************
set overwrite=false
echo overwrite: %overwrite%
echo storepath: %storepath%


A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=mgmt -orchtoscpserverip=%OrchToScpServerIp% -a10toscpserverip=%A10ToScpServerIp% -scpusername=%ScpUserName% -scppassword=%ScpPassword% -scpport=%ScpPort%


echo:
echo ******************************************************************************************************************
echo TC13 Remove certificate bound to Management Port should remove the cert and leave the binding in place
echo ******************************************************************************************************************
echo overwrite: %overwrite%
echo storepath: %storepath%
set mgt=remove

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=mgmt -orchtoscpserverip=%OrchToScpServerIp% -a10toscpserverip=%A10ToScpServerIp% -scpusername=%ScpUserName% -scppassword=%ScpPassword% -scpport=%ScpPort%

:TC14

echo:
echo ******************************************************************************************************************
echo TC14 Inventory Management Certificates from SCP Location
echo ******************************************************************************************************************
echo overwrite: %overwrite%
set storepath=/home/ec2-user
echo storepath: %storepath%
set casename=Inventory

A10vThunderTestConsole.exe -clientmachine=%clientmachine% -casename=%casename% -user=%user% -password=%password% -storepath=%storepath% -devicegroup= -managementtype=%mgt% -certalias=%cert% -overwrite=%overwrite% -storetype=mgmt -orchtoscpserverip=%OrchToScpServerIp% -a10toscpserverip=%A10ToScpServerIp% -scpusername=%ScpUserName% -scppassword=%ScpPassword% -scpport=%ScpPort%

@pause
