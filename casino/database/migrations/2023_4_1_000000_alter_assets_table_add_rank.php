<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   2023_4_1_000000_alter_assets_table_add_rank.php
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AlterAssetsTableAddRank extends Migration
{
    
    public function up()
    {
        Schema::table('assets', function (Blueprint $table) {
            $table->unsignedInteger('rank')->after('name');
            $table->index('rank');
        });
    }

    
    public function down()
    {
        Schema::table('assets', function (Blueprint $table) {
            $table->dropColumn('rank');
        });
    }
}
