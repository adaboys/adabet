<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   filename.php
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AlterUsersTableAddNotes extends Migration
{
    
    public function up()
    {
        Schema::table('users', function (Blueprint $table) {
            $table->text('notes')->nullable()->after('permissions');
        });
    }

    
    public function down()
    {
        Schema::table('users', function (Blueprint $table) {
            $table->dropColumn('notes');
        });
    }
}
